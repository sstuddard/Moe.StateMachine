using System;
using System.Collections.Generic;
using Moe.StateMachine.Actions;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine
{
	public class StateBuilder
	{
		private State state;
		private StateMachine stateMachine;
		private StateActions stateActions;

		public StateBuilder(StateMachine stateMachine, object stateId, State parent)
		{
			this.stateMachine = stateMachine;
			this.stateActions = new StateActions(stateId);
			this.state = CreateState(stateId, parent);
		}

		public StateBuilder(StateMachine stateMachine, State state)
		{
			this.state = state;
			this.stateActions = state.Actions;
			this.stateMachine = stateMachine;
		}

		public object Id { get { return state.Id; } }

		public StateBuilder this[object idx]
		{
			get
			{
				if (!state.ContainsState(idx))
					AddState(idx);
				return stateMachine.CreateStateBuilder(state.GetState(idx));
			}
		}

		public virtual State CreateState(object stateId, State parent)
		{
			return new State(stateId, parent, new StateActions(stateId));
		}

		#region Builder methods
		public StateBuilder DefaultTransition(object targetState)
		{
			return TransitionTo(StateMachine.DefaultEntryEvent, targetState);
		}

		public StateBuilder DefaultTransition(object targetState, Func<bool> guard)
		{
			return TransitionTo(StateMachine.DefaultEntryEvent, targetState, guard);
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
			stateMachine.CreateStateBuilder(state.Parent).DefaultTransition(state.Id);
			return this;
		}

		public StateBuilder OnEnter(Action<object> action)
		{
			stateActions.AddEnter(action);
			return this;
		}

		public StateBuilder OnEnter<T>(Action<object,T> action)
		{
			stateActions.AddEnter(action);
			return this;
		}

		public StateBuilder OnExit(Action<object> action)
		{
			stateActions.AddExit(action);
			return this;
		}

		public StateBuilder OnExit<T>(Action<object, T> action)
		{
			stateActions.AddExit(action);
			return this;
		}

		public StateBuilder AddState(object newStateId)
		{
			if (stateMachine.GetState(newStateId) != null)
				throw new InvalidOperationException();

			State newState = CreateState(newStateId, state);
			state.AddChildState(newState);

			return stateMachine.CreateStateBuilder(newState);
		}
		#endregion
	}
}