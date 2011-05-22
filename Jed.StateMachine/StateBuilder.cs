using System;
using System.Collections.Generic;

namespace Jed.StateMachine
{
	public class StateBuilder
	{
		private State state;
		private StateMachine stateMachine;

		internal StateBuilder(StateMachine stateMachine, State state)
		{
			this.state = state;
			this.stateMachine = stateMachine;
		}

		public object Id { get { return state.Id; } }

		public static implicit operator State(StateBuilder builder)
		{
			return builder.state;
		}

		public StateBuilder DefaultTransition(object targetState)
		{
			return TransitionTo(stateMachine.DefaultEntryEvent, targetState);
		}

		public StateBuilder TransitionTo(object eventTarget, object targetState)
		{
			state.AddTransition(eventTarget, new StateLocator(targetState, stateMachine));
			return this;
		}

		public StateBuilder InitialState()
		{
			new StateBuilder(stateMachine, state.Parent).DefaultTransition(state.Id);
			return this;
		}

		public StateBuilder OnEnter(Action<object> action)
		{
			state.AddEnterAction(action);
			return this;
		}

		public StateBuilder AddState(object newState)
		{
			if (stateMachine.GetState(newState) != null)
				throw new InvalidOperationException();

			return new StateBuilder(stateMachine, state.AddChildState(newState));
		}
	}
}