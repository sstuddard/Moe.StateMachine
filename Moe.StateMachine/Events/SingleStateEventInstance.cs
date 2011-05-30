using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Events
{
	public class SingleStateEventInstance : EventInstance
	{
		private State targetState;
		
		public SingleStateEventInstance(State targetState, object eventTarget)
			: base(eventTarget)
		{
			this.targetState = targetState;
		}

		public override bool MatchesTransition(Transition transition)
		{
			return base.MatchesTransition(transition) && transition.SourceState.Equals(targetState);
		}
	}
}
