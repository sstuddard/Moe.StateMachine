using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class DefaultEventEntryInstance : EventInstance
	{
		public DefaultEventEntryInstance()
			: base(StateMachine.DefaultEntryEvent)
		{
		}

		public override TransitionInstance ProcessEvent(State current)
		{
			return current.EvaluateEvent(eventPosted);
		}
	}
}
