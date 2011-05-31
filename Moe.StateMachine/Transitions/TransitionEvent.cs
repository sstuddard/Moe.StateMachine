using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Transitions
{
	public class TransitionEvent
	{
		private State sourceState;
		private Transition transition;
		private EventInstance eventInstance;

		public TransitionEvent(State sourceState, Transition transition, EventInstance eventInstance)
		{
			this.sourceState = sourceState;
			this.transition = transition;
			this.eventInstance = eventInstance;
		}

		public State SourceState { get { return sourceState; } }
		public State TargetState { get { return transition.TargetState; } }
		public EventInstance EventInstance { get { return eventInstance; } }
	}
}
