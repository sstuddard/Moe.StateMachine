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

		public virtual Transition CreateTransition(TransitionSpec spec)
		{
			return spec.CreateTransition(Context);
		}

		#region Builder methods
		public virtual IStateBuilder DefaultTransition(object targetState)
		{
			return TransitionTo(StateMachine.DefaultEntryEvent, targetState);
		}

		public virtual IStateBuilder DefaultTransition(object targetState, Func<bool> guard)
		{
			return TransitionTo(StateMachine.DefaultEntryEvent, targetState, guard);
		}

		public virtual IStateBuilder TransitionTo(object eventTarget, object targetState)
		{
			var transitionSpec = new TransitionSpec(Id, eventTarget, targetState);
			secondPassActions.Add(s => s.AddTransition(transitionSpec.CreateTransition(Context)));
			return this;
		}

		public virtual IStateBuilder TransitionTo(object eventTarget, object targetState, Func<bool> guard)
		{
			var transitionSpec = new GuardedTransitionSpec(Id, eventTarget, targetState, guard);
			secondPassActions.Add(s => s.AddTransition(transitionSpec.CreateTransition(Context)));
			return this;
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

	public class TransitionSpec
	{
		public object SourceState { get; private set; }
		public object Event { get; private set; }
		public object TargetState { get; private set; }

		public TransitionSpec(object source, object eventType, object target)
		{
			SourceState = source;
			Event = eventType;
			TargetState = target;
		}

		public virtual Transition CreateTransition(IStateBuilderContext context)
		{
			return new Transition(context.Resolve(SourceState), Event, context.Resolve(TargetState));
		}
	}

	public class GuardedTransitionSpec : TransitionSpec
	{
		public Func<bool> Guard { get; private set; }

		public GuardedTransitionSpec(object source, object eventType, object target, Func<bool> guard)
			: base(source, eventType, target)
		{
			Guard = guard;
		}

		public override Transition CreateTransition(IStateBuilderContext context)
		{
			return new GuardedTransition(context.Resolve(SourceState), Event, context.Resolve(TargetState), Guard);
		}
	}
}