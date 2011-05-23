using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	public class EventInstance
	{
		private object eventTarget;
		
		public EventInstance(object eventTarget)
		{
			this.eventTarget = eventTarget;
		}

		public virtual bool MatchesTransition(Transition transition)
		{
			return eventTarget.Equals(transition.EventTarget);
		}
	}
}
