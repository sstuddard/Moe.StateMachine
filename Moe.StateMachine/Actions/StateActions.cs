using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Actions
{
	public class StateActions
	{
		private enum ActionType
		{
			Enter,
			Exit
		}

		private Dictionary<ActionType, List<IActionPerformer>> actions;

		public StateActions()
		{
			actions = new Dictionary<ActionType, List<IActionPerformer>>();
			actions[ActionType.Enter] = new List<IActionPerformer>();
			actions[ActionType.Exit] = new List<IActionPerformer>();
		}

		public void AddExit(Action<TransitionReceipt> action)
		{
			actions[ActionType.Exit].Add(new SimpleAction(action));
		}

		public void AddEnter(Action<TransitionReceipt> action)
		{
			actions[ActionType.Enter].Add(new SimpleAction(action));
		}

		public void PerformEnter(TransitionEvent transitionEvent)
		{
			actions[ActionType.Enter].ForEach(a => a.Perform(new TransitionReceipt(transitionEvent)));
		}

		public void PerformExit(TransitionEvent transitionEvent)
		{
			actions[ActionType.Exit].ForEach(a => a.Perform(new TransitionReceipt(transitionEvent)));
		}
	}
}
