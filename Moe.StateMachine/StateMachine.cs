using System;
using Moe.StateMachine.Actions;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
    public class StateMachine : IDisposable
    {
		public static readonly object DefaultEntryEvent = "DefaultEntry";
		public static readonly object TimeoutEvent = "Timeout";
    	public static readonly object PulseEvent = "Pulse";

    	protected State current;
		protected RootState root;
		protected IStateBuilder rootBuilder;
		protected EventProcessor eventHandler;
    	protected StateActionDirector stateActions;
		protected TimerManager timers;

		public StateMachine()
		{
			root = new RootState();
			rootBuilder = this.CreateStateBuilder(root);
			eventHandler = new EventProcessor();
			timers = new TimerManager();
			stateActions = new StateActionDirector();
		}

		/// <summary>
		/// Short circuit indexer.  This will fetch ANY state by id if it exists.
		/// If it doesn't exist, it will be created off the root state.  
		/// Statebuilder does NOT work the same way.
		/// </summary>
		/// <param name="idx">State ID</param>
		/// <returns></returns>
    	public IStateBuilder this[object idx] 
		{ 
			get
			{
				if (root.GetState(idx) != null)
					return CreateStateBuilder(root.GetState(idx));
				return rootBuilder[idx];
			}
		}

		public StateActionDirector StateActions { get { return stateActions; } }
		public RootState RootNode { get { return root; } }
		public State CurrentState { get { return current; } }

		/// <summary>
		/// Extension point for creating own builders
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public virtual IStateBuilder CreateStateBuilder(State state)
		{
			return new StateBuilder(this, state);
		}

		/// <summary>
		/// Returns bool indicating if the machine is in the given state (at any level)
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public virtual bool InState(object state)
		{
			bool result = false;
			if (current != null)
				current.VisitParentChain(s => result |= s.Id.Equals(state));
			return result;
		}

		/// <summary>
		/// Starts the finite state machine
		/// </summary>
    	public virtual void Start()
    	{
    		current = root.ProcessEvent(root, new SingleStateEventInstance(root, DefaultEntryEvent));
			if (current == null || current == root)
				throw new InvalidOperationException("No initial state found.");
		}

		/// <summary>
		/// Sends a pulse event.  Primarily useful in a synchronous state machine with timers.
		/// </summary>
		public virtual void Pulse()
		{
			PostEvent(PulseEvent);
		}

		/// <summary>
		/// Post an event to the state machine
		/// </summary>
		/// <param name="eventToPost"></param>
		public virtual void PostEvent(object eventToPost)
		{
			PostEvent(new EventInstance(eventToPost));
		}

		protected virtual void PostEvent(EventInstance eventToPost)
		{
			UpdateTimers();

			eventHandler.AddEvent(eventToPost);

			while (eventHandler.CanProcess)
				current = eventHandler.ProcessNextEvent(current);
		}

		public virtual void RegisterTimer(State s, DateTime timeout)
		{
			timers.SetTimer(s, timeout);
		}

		public virtual void RemoveTimer(State s)
		{
			timers.ClearTimer(s);
		}

		protected virtual void UpdateTimers()
		{
			// Only grab one timeout at a time, a flurry of timeouts isn't helpful
			State timeout = timers.GetNextStateTimeout();
			if (timeout != null)
				eventHandler.AddEvent(new SingleStateEventInstance(timeout, TimeoutEvent));
		}

		#region StateBuilder forwarding and help
		public IStateBuilder AddState(object identifier)
		{
			return rootBuilder.AddState(identifier);
		}

		public IStateBuilder DefaultTransition(object targetState)
		{
			return rootBuilder.DefaultTransition(targetState);
		}

		public IStateBuilder DefaultTransition(object targetState, Func<bool> guard)
		{
			return rootBuilder.DefaultTransition(targetState, guard);
		}

		internal State GetState(object identifier)
		{
			return root.GetState(identifier);
		}
		#endregion

		public virtual void Dispose()
		{
		}
	}
}
