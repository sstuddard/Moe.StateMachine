using System;
using Moe.StateMachine.Extensions.Actions;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Extensions.StateWatcher
{
	public class StateWatcherAction : IDisposable
	{
		public event EventHandler Performed;
		private EnterAction enterAction;

		public StateWatcherAction(State state)
		{
			State = state;
			enterAction = new EnterAction(State, Perform);
		}

		public State State { get; private set; }

		public void Perform(TransitionReceipt transitionReceipt)
		{
			if (Performed != null)
				Performed(this, new EventArgs());
		}

		public void Dispose()
		{
			enterAction.Dispose();
		}
	}
}
