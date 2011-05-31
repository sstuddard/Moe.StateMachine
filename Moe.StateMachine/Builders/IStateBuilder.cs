using System;
using System.Collections.Generic;
using Moe.StateMachine.Builders;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public interface IStateBuilder : IBaseStateBuilder
	{
		IEnumerable<IBaseStateBuilder> SubStates { get; }
		IStateBuilderContext Context { get; }

		void AddSecondPassAction(Action<State> action);

		// State building
		IStateBuilder AddState(object stateId);
		IStateBuilder AddHistory();
		IStateBuilder this[object stateId] { get; }

		// Transition support
		ITransitionBuilder DefaultTransition(object targetState);
		ITransitionBuilder TransitionOn(object eventTarget);
		ITransitionBuilder TransitionOn(object eventTarget, object targetState);
		IStateBuilder InitialState();
	}
}
