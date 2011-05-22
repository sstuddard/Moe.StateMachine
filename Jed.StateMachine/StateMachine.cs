using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
    public class StateMachine
    {
		internal static readonly object DefaultEntryEvent = "DefaultEntry";
		internal static readonly object TimeoutEvent = "Timeout";

    	private State current;
    	private RootState root;
    	private StateBuilder rootBuilder;
    	private EventProcessor eventHandler;
    	private TimerManager timers;

		public StateMachine()
		{
			root = new RootState(this);
			rootBuilder = new StateBuilder(this, root);
			eventHandler = new EventProcessor();
			timers = new TimerManager();
		}

    	public StateBuilder this[object idx] 
		{ 
			get { return new StateBuilder(this, root.GetState(idx)); }
		}

		internal State CurrentState { get { return current; } }

		public bool InState(object state)
		{
			bool result = false;
			if (current != null)
				current.VisitParentChain(s => result |= s.Id.Equals(state));
			return result;
		}

    	public virtual void Start()
    	{
    		current = root.ProcessEvent(new SingleStateEventInstance(root, DefaultEntryEvent));
			if (current == null || current == root)
				throw new InvalidOperationException("No initial state found.");
		}

		public virtual void PostEvent(object eventToPost)
		{
			UpdateTimers();

			eventHandler.AddEvent(new EventInstance(eventToPost));

			while (eventHandler.CanProcess)
				current = eventHandler.ProcessNextEvent(current);
		}

		internal virtual void RegisterTimer(State s, DateTime timeout)
		{
			timers.SetTimer(s, timeout);
		}

		internal virtual void RemoveTimer(State s)
		{
			timers.ClearTimer(s);
		}

		protected virtual void UpdateTimers()
		{
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

		internal State GetState(object identifier)
		{
			return root.GetState(identifier);
		}
		#endregion
	}
}
