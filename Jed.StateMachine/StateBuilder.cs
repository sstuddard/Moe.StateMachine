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

		public StateBuilder DefaultTransition(object targetState)
		{
			return TransitionTo(StateMachine.DefaultEntryEvent, targetState);
		}

		public StateBuilder TransitionTo(object eventTarget, object targetState)
		{
			state.AddTransition(new Transition(state, eventTarget, new StateLocator(targetState, stateMachine)));
			return this;
		}

		public StateBuilder TransitionTo(object eventTarget, object targetState, Func<bool> guard)
		{
			state.AddTransition(new GuardedTransition(state, eventTarget, new StateLocator(targetState, stateMachine), guard));
			return this;
		}

		public StateBuilder Timeout(int timeoutInMilliseconds, object targetState)
		{
			Transition timeout = new Transition(state, StateMachine.TimeoutEvent, new StateLocator(targetState, stateMachine));
			return Timeout(timeoutInMilliseconds, timeout);
		}

		public StateBuilder Timeout(int timeoutInMilliseconds, object targetState, Func<bool> guard)
		{
			Transition timeout = 
				new GuardedTransition(state, StateMachine.TimeoutEvent, new StateLocator(targetState, stateMachine), guard);
			return Timeout(timeoutInMilliseconds, timeout);
		}

		private StateBuilder Timeout(int timeoutInMilliseconds, Transition transition)
		{
			state.AddTransition(transition);
			OnEnter(s => stateMachine.RegisterTimer(state, DateTime.Now.AddMilliseconds(timeoutInMilliseconds)));
			OnExit(s => stateMachine.RemoveTimer(state));
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

		public StateBuilder OnExit(Action<object> action)
		{
			state.AddExitAction(action);
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