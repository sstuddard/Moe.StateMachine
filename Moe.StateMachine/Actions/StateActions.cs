using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Actions
{
	public class StateActions
	{
		private enum ActionType
		{
			Enter,
			Exit
		}

		private object stateId;
		private Dictionary<ActionType, List<IActionPerformer>> actions;

		public StateActions(object stateId)
		{
			this.stateId = stateId;

			actions = new Dictionary<ActionType, List<IActionPerformer>>();
			actions[ActionType.Enter] = new List<IActionPerformer>();
			actions[ActionType.Exit] = new List<IActionPerformer>();
		}

		public void AddExit(Action<object> action)
		{
			actions[ActionType.Exit].Add(new SimpleAction(action));
		}

		public void AddExit<T>(Action<object, T> action)
		{
			actions[ActionType.Exit].Add(new ContextAction<T>(action));
		}

		public void AddEnter(Action<object> action)
		{
			actions[ActionType.Enter].Add(new SimpleAction(action));
		}

		public void AddEnter<T>(Action<object,T> action)
		{
			actions[ActionType.Enter].Add(new ContextAction<T>(action));
		}

		public void PerformEnter(EventInstance eventInstance)
		{
			actions[ActionType.Enter].ForEach(a => a.Perform(stateId, eventInstance.Context));
		}

		public void PerformExit(EventInstance eventInstance)
		{
			actions[ActionType.Exit].ForEach(a => a.Perform(stateId, eventInstance.Context));
		}
	}
}
