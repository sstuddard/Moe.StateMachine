using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Actions
{
	public class TransitionReceipt
	{
		private TransitionEvent transitionEvent;

		public TransitionReceipt(TransitionEvent transitionEvent)
		{
			this.transitionEvent = transitionEvent;
		}

		public object Event { get { return transitionEvent.EventInstance.Event; } }
	}
}
