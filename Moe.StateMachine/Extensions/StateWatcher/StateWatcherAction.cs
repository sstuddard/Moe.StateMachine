using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Actions;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Extensions.StateWatcher
{
	public class StateWatcherAction : IActionPerformer
	{
		public event EventHandler Performed;

		public StateWatcherAction(State state)
		{
			State = state;
		}

		public State State { get; private set; }
		public ActionType Type { get { return ActionType.Enter; } }

		public void Perform(TransitionReceipt transitionReceipt)
		{
			if (Performed != null)
				Performed(this, new EventArgs());
		}
	}
}
