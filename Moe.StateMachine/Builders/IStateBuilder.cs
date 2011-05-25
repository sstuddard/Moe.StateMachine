using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Actions;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public interface IStateBuilder
	{
		object Id { get; }
		State State { get; }
		StateMachine StateMachine { get; }
		IStateBuilder this[object idx] { get; }
		IStateBuilder AddState(object newStateId);

		// Transition support
		IStateBuilder DefaultTransition(object targetState);
		IStateBuilder DefaultTransition(object targetState, Func<bool> guard);
		IStateBuilder TransitionTo(object eventTarget, object targetState);
		IStateBuilder TransitionTo(object eventTarget, object targetState, Func<bool> guard);
		IStateBuilder InitialState();

		// Timer support
		IStateBuilder Timeout(int timeoutInMilliseconds, object targetState);
		IStateBuilder Timeout(int timeoutInMilliseconds, object targetState, Func<bool> guard);

		// Action support
		IStateBuilder OnEnter(Action<TransitionReceipt> action);
		IStateBuilder OnExit(Action<TransitionReceipt> action);
	}
}
