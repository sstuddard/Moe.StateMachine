using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.States
{
	public class StateEventArgs : EventArgs
	{
		public StateEventArgs(TransitionEvent transitionEvent)
		{
			TransitionEvent = transitionEvent;
		}

		public TransitionEvent TransitionEvent { get; private set; }
	}
}
