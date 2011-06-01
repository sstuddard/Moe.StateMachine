using System;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Builders
{
	public class HistoryStateBuilder : IBaseStateBuilder
	{
		private readonly object id;
		private readonly HistoryState.HistoryFetchStrategy historyStrategy;

		protected HistoryStateBuilder(IStateBuilder parent, HistoryState.HistoryFetchStrategy historyStrategy)
		{
			this.id = new HistoryStateId(parent.Id);
			this.historyStrategy = historyStrategy;
		}

		public object Id { get { return id; } }

		public State Build(State parent)
		{
			HistoryState state = new HistoryState(parent, historyStrategy);

			return state;
		}

		public static IBaseStateBuilder CreateShallowHistoryBuilder(IStateBuilder parent)
		{
			return new HistoryStateBuilder(parent, HistoryState.GetShallowState);
		}

		public static IBaseStateBuilder CreateDeepHistoryBuilder(IStateBuilder parent)
		{
			return new HistoryStateBuilder(parent, HistoryState.GetDeepState);
		}
	}
}
