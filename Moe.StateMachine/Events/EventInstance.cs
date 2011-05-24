using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Events
{
	public class EventInstance
	{
		private object eventTarget;
		private object context;
		
		public EventInstance(object eventTarget)
		{
			this.eventTarget = eventTarget;
		}

		public EventInstance(object eventTarget, object context)
		{
			this.eventTarget = eventTarget;
			this.context = context;
		}

		public object Context { get { return this.context; } }

		public virtual bool MatchesTransition(Transition transition)
		{
			return eventTarget.Equals(transition.EventTarget);
		}
	}
}
