using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Actions
{
	public class StateActions
	{
		private State state;
		private List<IActionPerformer> actions;

		public StateActions(State state)
		{
			this.state = state;
			this.state.Entered += OnStateEntered;
			this.state.Exited += OnStateExited;

			actions = new List<IActionPerformer>();
		}

		public void AddAction(IActionPerformer action)
		{
			actions.Add(action);
		}

		public void RemoveAction(IActionPerformer action)
		{
			actions.Remove(action);
		}

		public bool ContainsAction(IActionPerformer action)
		{
			return actions.Contains(action);
		}

		private void OnStateEntered(object sender, StateEventArgs args)
		{
			IEnumerable<IActionPerformer> toPerform = 
				new List<IActionPerformer>(actions.Where(ap => ap.Type == ActionType.Enter));
			
			foreach (IActionPerformer action in toPerform)
				action.Perform(new TransitionReceipt(args.TransitionEvent));
		}

		private void OnStateExited(object sender, StateEventArgs args)
		{
			IEnumerable<IActionPerformer> toPerform = 
				new List<IActionPerformer>(actions.Where(ap => ap.Type == ActionType.Exit));
			
			foreach (IActionPerformer action in toPerform)
				action.Perform(new TransitionReceipt(args.TransitionEvent));
		}
	}
}
