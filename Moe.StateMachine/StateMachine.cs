using System;
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
		protected StateBuilder rootBuilder;
		protected EventProcessor eventHandler;
		protected TimerManager timers;

		public StateMachine()
		{
			root = new RootState(this);
			rootBuilder = new StateBuilder(this, root);
			eventHandler = new EventProcessor();
			timers = new TimerManager();
		}

    	public StateBuilder this[object idx] 
		{ 
			get { return rootBuilder[idx]; }
		}

		public RootState RootNode { get { return root; } }
		public State CurrentState { get { return current; } }

		public bool InState(object state)
		{
			bool result = false;
			if (current != null)
				current.VisitParentChain(s => result |= s.Id.Equals(state));
			return result;
		}

    	public virtual void Start()
    	{
    		current = root.ProcessEvent(root, new SingleStateEventInstance(root, DefaultEntryEvent));
			if (current == null || current == root)
				throw new InvalidOperationException("No initial state found.");
		}

		public virtual void Pulse()
		{
			PostEvent(PulseEvent);
		}

		public virtual void PostEvent(object eventToPost)
		{
			PostEvent(new EventInstance(eventToPost));
		}

		public virtual void PostEvent<T>(object eventToPost, T context)
		{
			PostEvent(new EventInstance(eventToPost, context));
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
		public StateBuilder AddState(object identifier)
		{
			return rootBuilder.AddState(identifier);
		}

		public StateBuilder DefaultTransition(object targetState)
		{
			return rootBuilder.DefaultTransition(targetState);
		}

		public StateBuilder DefaultTransition(object targetState, Func<bool> guard)
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
