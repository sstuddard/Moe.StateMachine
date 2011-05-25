using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Builders
{
	public class TransitionBuilder : StateBuilderSkeleton
	{
		public TransitionBuilder(IStateBuilder stateBuilder)
			: base(stateBuilder)
		{
		}

		public override IStateBuilder DefaultTransition(object targetState)
		{
			return TransitionTo(StateMachine.DefaultEntryEvent, targetState);
		}

		public override IStateBuilder DefaultTransition(object targetState, Func<bool> guard)
		{
			return TransitionTo(StateMachine.DefaultEntryEvent, targetState, guard);
		}

		public override IStateBuilder TransitionTo(object eventTarget, object targetState)
		{
			State state = sourceBuilder.State;
			state.AddTransition(
				new Transition(state, eventTarget, new StateLocator(targetState, sourceBuilder.StateMachine)));
			return sourceBuilder;
		}

		public override IStateBuilder TransitionTo(object eventTarget, object targetState, Func<bool> guard)
		{
			State state = sourceBuilder.State;
			state.AddTransition(
				new GuardedTransition(state, eventTarget, new StateLocator(targetState, sourceBuilder.StateMachine), guard));
			return sourceBuilder;
		}

		public override IStateBuilder InitialState()
		{
			State state = sourceBuilder.State;
			sourceBuilder.StateMachine.CreateStateBuilder(state.Parent).DefaultTransition(state.Id);
			return sourceBuilder;
		}
	}
}
