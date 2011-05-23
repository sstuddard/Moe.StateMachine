using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine
{
	public class GuardedTransition : Transition
	{
		private Func<bool> guard;

		public GuardedTransition(State sourceState, object eventTarget, StateLocator targetState, Func<bool> guard)
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
