using System;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.States
{
	public class StateTransitionEventArgs : EventArgs
	{
		public StateTransitionEventArgs(TransitionEvent transitionEvent)
		{
			TransitionEvent = transitionEvent;
		}

		public TransitionEvent TransitionEvent { get; private set; }
	}
}
