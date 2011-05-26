using System;
using System.Collections.Generic;
using Moe.StateMachine.Actions;
using Moe.StateMachine.Builders;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine
{
	public class StateBuilder : IStateBuilder
	{
		private readonly State state;
		private readonly StateMachine stateMachine;

		public StateBuilder(StateMachine stateMachine, object stateId, State parent)
		{
			this.stateMachine = stateMachine;
			this.state = CreateState(stateId, parent);
		}

		public StateBuilder(StateMachine stateMachine, State state)
		{
			this.state = state;
			this.stateMachine = stateMachine;
		}

		public object Id { get { return state.Id; } }
		public State State { get { return state; } }
		public StateMachine StateMachine { get { return stateMachine; } }

		public static implicit operator State(StateBuilder builder)
		{
			return builder.State;
		}

		public virtual IStateBuilder this[object idx]
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
			return new State(stateId, parent);
		}

		#region Builder methods
		public virtual IStateBuilder DefaultTransition(object targetState)
		{
			return new TransitionBuilder(this).DefaultTransition(targetState);
		}

		public virtual IStateBuilder DefaultTransition(object targetState, Func<bool> guard)
		{
			return new TransitionBuilder(this).DefaultTransition(targetState, guard);
		}

		public virtual IStateBuilder TransitionTo(object eventTarget, object targetState)
		{
			return new TransitionBuilder(this).TransitionTo(eventTarget, targetState);
		}

		public virtual IStateBuilder TransitionTo(object eventTarget, object targetState, Func<bool> guard)
		{
			return new TransitionBuilder(this).TransitionTo(eventTarget, targetState, guard);
		}

		public virtual IStateBuilder InitialState()
		{
			return new TransitionBuilder(this).InitialState();
		}

		public virtual IStateBuilder Timeout(int timeoutInMilliseconds, object targetState)
		{
			return new TimerBuilder(this).Timeout(timeoutInMilliseconds, targetState);
		}

		public virtual IStateBuilder Timeout(int timeoutInMilliseconds, object targetState, Func<bool> guard)
		{
			return new TimerBuilder(this).Timeout(timeoutInMilliseconds, targetState, guard);
		}

		public virtual IStateBuilder OnEnter(Action<TransitionReceipt> action)
		{
			return new ActionBuilder(this).OnEnter(action);
		}

		public virtual IStateBuilder OnExit(Action<TransitionReceipt> action)
		{
			return new ActionBuilder(this).OnExit(action);
		}

		public virtual IStateBuilder AddState(object newStateId)
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