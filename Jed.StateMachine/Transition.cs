using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class Transition
	{
		private object eventTarget;
		private StateLocator targetState;

		public Transition(object eventTarget, StateLocator targetState)
		{
			this.eventTarget = eventTarget;
			this.targetState = targetState;
		}

		public State TargetState { get { return targetState; } }

		public bool Matches(object eventToMatch)
		{
			return eventTarget.Equals(eventToMatch);
		}
	}
}
