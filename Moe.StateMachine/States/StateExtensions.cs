using System;

namespace Moe.StateMachine.States
{
	public static class StateExtensions
	{
		public static void VisitChildren(this State s, Action<State> action)
		{
			foreach (State child in s.Substates)
			{
				action(child);
				child.VisitChildren(action);
			}
		}

		public static State GetState(this State s, object stateId)
		{
			if (s.Id == stateId)
				return s;
			else
			{
				State state = null;
				VisitChildren(s, z =>
				                 	{
										if (z.Id.Equals(stateId))
											state = z;
				                 	});

				return state;
			}
		}

		public static bool ContainsState(this State s, State state)
		{
			return s.GetState(state.Id) != null;
		}

		public static void VisitParentChain(this State s, Action<State> action)
		{
			if (s == null)
				return;

			action(s);
			VisitParentChain(s.Parent, action);
		}

		public static State GetSubstatePath(this State s, State targetState)
		{
			foreach (State substate in s.Substates)
			{
				if (substate.ContainsState(targetState))
					return substate;
			}

			return null;
		}
	}
}
