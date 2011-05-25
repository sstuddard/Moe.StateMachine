using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Actions;

namespace Moe.StateMachine.Builders
{
	public class ActionBuilder : StateBuilderSkeleton
	{
		public ActionBuilder(IStateBuilder source)
			: base(source)
		{
		}

		public override IStateBuilder OnEnter(Action<TransitionReceipt> action)
		{
			State.Actions.AddEnter(action);
			return sourceBuilder;
		}

		public override IStateBuilder OnExit(Action<TransitionReceipt> action)
		{
			State.Actions.AddExit(action);
			return sourceBuilder;
		}
	}
}
