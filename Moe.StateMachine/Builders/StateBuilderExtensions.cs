using System;

namespace Moe.StateMachine.Builders
{
	public static class StateBuilderExtensions
	{
		public static void VisitChildren(this IStateBuilder s, Action<IStateBuilder> action)
		{
			foreach (IStateBuilder child in s.SubStates)
			{
				action(child);
				child.VisitChildren(action);
			}
		}

		public static IStateBuilder GetState(this IStateBuilder s, object stateId)
		{
			if (s.Id == stateId)
				return s;
			else
			{
				IStateBuilder state = null;
				VisitChildren(s, z =>
				{
					if (z.Id.Equals(stateId))
						state = z;
				});

				return state;
			}
		}

		public static bool ContainsState(this IStateBuilder s, object stateId)
		{
			return s.GetState(stateId) != null;
		}
	}
}
