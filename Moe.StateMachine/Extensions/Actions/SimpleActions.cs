using System;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Extensions.Actions
{
	public class EnterAction : IDisposable
	{
		public Action<TransitionReceipt> Action { get; private set; }
		private State state;

		public EnterAction(State state, Action<TransitionReceipt> action)
		{
			this.state = state;
			this.state.Entered += OnStateEntered;
			this.Action = action;
		}

		private void OnStateEntered(object sender, StateTransitionEventArgs args)
		{
			Action(new TransitionReceipt(args.TransitionEvent));
		}

		public void Dispose()
		{
			this.state.Entered -= OnStateEntered;
		}
	}

	public class ExitAction : IDisposable
	{
		public Action<TransitionReceipt> Action { get; private set; }
		private State state;

		public ExitAction(State state, Action<TransitionReceipt> action)
		{
			this.state = state;
			this.state.Exited += OnStateExited;
			this.Action = action;
		}

		private void OnStateExited(object sender, StateTransitionEventArgs args)
		{
			Action(new TransitionReceipt(args.TransitionEvent));
		}

		public void Dispose()
		{
			this.state.Exited -= OnStateExited;
		}
	}
}
