using System;
using System.Collections.Generic;
using Moe.StateMachine.Builders;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine
{
	public class StateBuilder : IStateBuilder
	{
		protected readonly List<IStateBuilder> substates;
		protected readonly List<Action<State>> secondPassActions;
		protected State state;

		public StateBuilder(IStateBuilder parent, object stateId, IStateBuilderContext context)
		{
			Parent = parent;
			Id = stateId;
			Context = context;

			substates = new List<IStateBuilder>();
			secondPassActions = new List<Action<State>>();
		}

		public virtual State Build(State parent)
		{
			// Build is called twice, first time around we build just the state
			if (state == null)
			{
				state = CreateState(Id, parent);

				foreach (IStateBuilder substate in substates)
				{
					state.AddChildState(substate.Build(state));
				}
			}
			else
			{
				foreach (Action<State> action in secondPassActions)
				{
					action(state);
				}
			}

			return state;
		}

		public IStateBuilderContext Context { get; private set; }
		public object Id { get; private set; }
		public IStateBuilder Parent { get; private set; }
		public IEnumerable<IStateBuilder> SubStates { get { return new List<IStateBuilder>(substates); } }

		public void AddSecondPassAction(Action<State> action)
		{
			secondPassActions.Add(action);
		}

		/// <summary>
		/// Short circuit indexer.  This will fetch ANY state by id if it exists.
		/// If it doesn't exist, it will be created off the root state.  
		/// Statebuilder does NOT work the same way.
		/// </summary>
		/// <param name="idx">State ID</param>
		/// <returns></returns>
		public virtual IStateBuilder this[object idx]
		{
			get
			{
				if (!this.ContainsState(idx))
					AddState(idx);
				return this.GetState(idx);
			}
		}

		public virtual State CreateState(object stateId, State parent)
		{
			if (parent == null)
				throw new InvalidOperationException("Parent may not be null for state: " + stateId);
			return new State(stateId, parent);
		}

		#region Builder methods
		public virtual ITransitionBuilder DefaultTransition(object targetState)
		{
			return new TransitionBuilder(this, StateMachine.DefaultEntryEvent, targetState);
		}

		public virtual ITransitionBuilder TransitionOn(object eventTarget)
		{
			return new TransitionBuilder(this, eventTarget);
		}

		public virtual ITransitionBuilder TransitionOn(object eventTarget, object targetState)
		{
			return new TransitionBuilder(this, eventTarget, targetState);
		}

		public virtual IStateBuilder InitialState()
		{
			Parent.DefaultTransition(Id);
			return this;
		}

		public virtual IStateBuilder AddState(object newStateId)
		{
			if (this.GetState(newStateId) != null)
				throw new InvalidOperationException();

			IStateBuilder newState = CreateStateBuilder(newStateId, this);
			substates.Add(newState);

			return newState;
		}

		public virtual IStateBuilder CreateStateBuilder(object stateId, IStateBuilder parent)
		{
			return new StateBuilder(parent, stateId, Context);
		}
		#endregion
	}
}