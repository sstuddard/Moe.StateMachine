using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine
{
	public class TransitionEvent
	{
		private Transition transition;
		private EventInstance eventInstance;

		public TransitionEvent(Transition transition, EventInstance eventInstance)
		{
			this.transition = transition;
			this.eventInstance = eventInstance;
		}

		public State TargetState { get { return transition.TargetState; } }
		public EventInstance EventInstance { get { return eventInstance; } }
	}
}
