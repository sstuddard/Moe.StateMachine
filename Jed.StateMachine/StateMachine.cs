﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
    public class StateMachine
    {
		internal static readonly object DefaultEntryEvent = new object();

    	private State current;
    	private State root;
    	private StateBuilder rootBuilder;
    	private EventProcessor eventHandler;

		public StateMachine()
		{
			root = new State("Root", null);
			rootBuilder = new StateBuilder(this, root);
			eventHandler = new EventProcessor();
		}

    	public StateBuilder this[object idx] 
		{ 
			get { return new StateBuilder(this, root.GetState(idx)); }
		}

		public bool InState(object state)
		{
			bool result = false;
			if (current != null)
				current.VisitParentChain(s => result |= s.Id.Equals(state));
			return result;
		}

    	public virtual void Start()
    	{
    		current = root;
    		PostEvent(DefaultEntryEvent);
			if (current == root)
				throw new InvalidOperationException();
		}

		public virtual void PostEvent(object eventToPost)
		{
			eventHandler.AddEvent(new EventInstance(eventToPost));

			while (eventHandler.CanProcess)
				current = eventHandler.ProcessNextEvent(current);
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
