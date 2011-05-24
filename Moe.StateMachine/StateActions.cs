using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public class StateActions
	{
		private enum ActionType
		{
			Enter,
			Exit
		}

		private State state;
		private Dictionary<ActionType, List<IActionPerformer>> actions;

		public StateActions(State state)
		{
			this.state = state;

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
			actions[ActionType.Enter].ForEach(a => a.Perform(state.Id, eventInstance.Context));
		}

		public void PerformExit(EventInstance eventInstance)
		{
			actions[ActionType.Exit].ForEach(a => a.Perform(state.Id, eventInstance.Context));
		}

		private interface IActionPerformer
		{
			void Perform(object stateId, object context);
		}

		private class SimpleAction : IActionPerformer
		{
			public Action<object> Action { get; private set; }

			public SimpleAction(Action<object> action)
			{
				Action = action;
			}

			public virtual void Perform(object stateId, object context)
			{
				Action(stateId);
			}
		}

		private class ContextAction<T> : IActionPerformer
		{
			public Action<object, T> Action { get; private set; }

			public ContextAction(Action<object, T> action)
			{
				Action = action;
			}

			public virtual void Perform(object stateId, object context)
			{
				Action(stateId, (T) context);
			}
		}
	}
}
