using System;
using System.Collections.Generic;
using Moe.StateMachine.Builders;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public interface IStateBuilder
	{
		object Id { get; }

		State Build(State parent);
		IEnumerable<IStateBuilder> SubStates { get; }
		IStateBuilderContext Context { get; }

		void AddSecondPassAction(Action<State> action);

		// State building
		IStateBuilder AddState(object stateId);
		IStateBuilder this[object stateId] { get; }

		// Transition support
		IStateBuilder DefaultTransition(object targetState);
		IStateBuilder DefaultTransition(object targetState, Func<bool> guard);
		IStateBuilder TransitionTo(object eventTarget, object targetState);
		IStateBuilder TransitionTo(object eventTarget, object targetState, Func<bool> guard);
		IStateBuilder InitialState();
	}
}
