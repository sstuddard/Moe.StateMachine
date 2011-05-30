using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Transitions
{
	public class Transition
	{
		private State sourceState;
		private object eventTarget;
		private State targetState;

		public Transition(State sourceState, object eventTarget, State targetState)
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
