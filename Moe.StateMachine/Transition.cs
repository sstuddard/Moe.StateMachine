using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine
{
	public class Transition
	{
		private State sourceState;
		private object eventTarget;
		private StateLocator targetState;

		public Transition(State sourceState, object eventTarget, StateLocator targetState)
		{
			this.sourceState = sourceState;
			this.eventTarget = eventTarget;
			this.targetState = targetState;
		}

		public State SourceState { get { return sourceState; } }
		public State TargetState { get { return targetState; } }
		public object EventTarget { get { return eventTarget; } }

		public virtual bool Matches(EventInstance eventToMatch)
		{
			return eventToMatch.MatchesTransition(this);
		}
	}
}
