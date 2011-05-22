using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal static class StateExtensions
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

		public static bool ContainsState(this State s, object stateId)
		{
			return s.GetState(stateId) != null;
		}

		public static void VisitParentChain(this State s, Action<State> action)
		{
			if (s == null)
				return;

			action(s);
			VisitParentChain(s.Parent, action);
		}
	}
}
