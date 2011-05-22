using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class GuardedTransition : Transition
	{
		private Func<bool> guard;

		public GuardedTransition(object eventTarget, StateLocator targetState, Func<bool> guard)
			: base(eventTarget, targetState)
		{
			this.guard = guard;
		}

		public override bool Matches(object eventToMatch)
		{
			return base.Matches(eventToMatch) && guard();
		}
	}
}
