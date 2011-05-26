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
		
		public EventInstance(object eventTarget)
		{
			this.eventTarget = eventTarget;
		}

		public object Event { get { return this.eventTarget; } }

		public virtual bool MatchesTransition(Transition transition)
		{
			return eventTarget.Equals(transition.EventTarget);
		}
	}
}
