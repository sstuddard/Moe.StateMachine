using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Transitions
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

		public State SourceState { get { return transition.SourceState; } }
		public State TargetState { get { return transition.TargetState; } }
		public EventInstance EventInstance { get { return eventInstance; } }
	}
}
