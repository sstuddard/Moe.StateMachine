using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	public class StateActions
	{
		private enum ActionType
		{
			Enter,
			Exit
		}

		private State state;
		private Dictionary<ActionType, List<Action<object>>> actions;

		public StateActions(State state)
		{
			this.state = state;

			actions = new Dictionary<ActionType, List<Action<object>>>();
			actions[ActionType.Enter] = new List<Action<object>>();
			actions[ActionType.Exit] = new List<Action<object>>();
		}

		public void AddExit(Action<object> action)
		{
			actions[ActionType.Exit].Add(action);
		}

		public void AddEnter(Action<object> action)
		{
			actions[ActionType.Enter].Add(action);
		}

		public void PerformEnter()
		{
			actions[ActionType.Enter].ForEach(a => a(state.Id));
		}

		public void PerformExit()
		{
			actions[ActionType.Exit].ForEach(a => a(state.Id));
		}
	}
}
