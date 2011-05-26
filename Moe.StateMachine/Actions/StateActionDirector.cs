using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Actions
{
	public class StateActionDirector
	{
		private Dictionary<State, StateActions> stateActions;

		public StateActionDirector()
		{
			stateActions = new Dictionary<State, StateActions>();
		}

		public void AddAction(State state, IActionPerformer action)
		{
			if (!stateActions.ContainsKey(state))
				stateActions[state] = new StateActions(state);
			stateActions[state].AddAction(action);
		}

		public void RemoveAction(IActionPerformer action)
		{
			foreach (State key in stateActions.Keys)
			{
				if (stateActions[key].ContainsAction(action))
					stateActions[key].RemoveAction(action);
			}
		}
	}
}
