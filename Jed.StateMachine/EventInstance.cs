using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class EventInstance
	{
		protected object eventPosted;

		public EventInstance(object eventPosted)
		{
			this.eventPosted = eventPosted;
		}

		public virtual TransitionInstance ProcessEvent(State current)
		{
			TransitionInstance transition = null;
			current.VisitParentChain(s =>
			{
				TransitionInstance found = s.EvaluateEvent(eventPosted);
				if (found != null && transition == null)
					transition = found;
			});

			return transition;
		}
	}
}
