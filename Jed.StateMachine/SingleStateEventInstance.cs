using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class SingleStateEventInstance : EventInstance
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
