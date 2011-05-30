using System;
using Moe.StateMachine.Builders;
using Moe.StateMachine.Extensions;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public class StateMachineBuilder : StateBuilder, IStateMachineInitializer
	{
		public StateMachineBuilder()
			: base(null, RootState.RootStateId, new StateBuilderContext())
		{
		}

		public State Initialize(StateMachine sm)
		{
			State root = Build(null);

			foreach (IPlugIn plugin in Context.PlugIns)
				sm.AddPlugIn(plugin);

			return root;
		}

		public override State Build(State parent)
		{
			RootState root = new RootState();
			((StateBuilderContext)Context).SetRootState(root);

			foreach (IStateBuilder substate in SubStates)
			{
				root.AddChildState(substate.Build(root));
			}

			foreach (Action<State> action in secondPassActions)
			{
				action(root);
			}

			this.VisitChildren(sb => sb.Build(root));

			return root;
		}
	}
}
