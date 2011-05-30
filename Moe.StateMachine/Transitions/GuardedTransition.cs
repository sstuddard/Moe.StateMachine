using System;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Transitions
{
	public class GuardedTransition : Transition
	{
		private Func<bool> guard;

		public GuardedTransition(State sourceState, object eventTarget, State targetState, Func<bool> guard)
			: base(sourceState, eventTarget, targetState)
		{
			this.guard = guard;
		}

		public override bool Matches(EventInstance eventToMatch)
		{
			return base.Matches(eventToMatch) && guard();
		}
	}
}
