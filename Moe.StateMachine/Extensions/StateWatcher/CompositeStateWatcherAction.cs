using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Actions;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Extensions.StateWatcher
{
	public class CompositeStateWatcherAction : IDisposable
	{
		private StateMachine stateMachine;
		private List<StateWatcherAction> stateActions;
		private State finishState;
		private Action<object> callback;

		public CompositeStateWatcherAction(
			StateMachine stateMachine, 
			IEnumerable<object> states,
			Action<object> callback)
		{
			this.stateMachine = stateMachine;
			this.stateActions = new List<StateWatcherAction>();
			this.finishState = null;
			this.callback = callback;
			
			foreach (object stateId in states)
			{
				State state = stateMachine[stateId].State;
				StateWatcherAction action = new StateWatcherAction(state);
				action.Performed += OnStateEntered;

				stateMachine.StateActions.AddAction(state, action);
				stateActions.Add(action);
			}
		}

		public void Cancel()
		{
			// Remove all actions, we're done
			foreach (StateWatcherAction action in stateActions)
			{
				stateMachine.StateActions.RemoveAction(action);
			}
		}

		private void OnStateEntered(object sender, EventArgs args)
		{
			if (finishState != null)
				return;

			StateWatcherAction watcher = sender as StateWatcherAction;
			finishState = watcher.State;

			Cancel();

			// Notify of the completion
			callback(finishState.Id);
		}

		public void Dispose()
		{
			Cancel();
		}
	}
}
