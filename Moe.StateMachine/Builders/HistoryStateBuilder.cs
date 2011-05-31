using System;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Builders
{
	public class HistoryStateBuilder : IBaseStateBuilder
	{
		private readonly object id;

		public HistoryStateBuilder(IStateBuilder parent)
		{
			id = new HistoryStateId(parent.Id);
		}

		public object Id { get { return id; } }

		public State Build(State parent)
		{
			HistoryState state = new HistoryState(parent);

			return state;
		}
	}
}
