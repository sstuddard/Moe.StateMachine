using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Builders
{
	public class TransitionBuilder : ITransitionBuilder
	{
		private readonly IStateBuilder stateBuilder;
		private readonly object eventId;
		private object targetStateId;
		private Func<bool> guard;

		public TransitionBuilder(IStateBuilder stateBuilder, object eventId)
		{
			this.eventId = eventId;
			this.stateBuilder = stateBuilder;

			stateBuilder.AddSecondPassAction(s =>
			                                 	{
			                                 		s.AddTransition(CreateTransition());
			                                 	});
		}

		public TransitionBuilder(IStateBuilder stateBuilder, object eventId, object targetStateId)
			: this(stateBuilder, eventId)
		{
			this.targetStateId = targetStateId;
		}

		public ITransitionBuilder To(object stateId)
		{
			if (targetStateId != null)
				throw new InvalidOperationException("Target state already specified for this transition");
			targetStateId = stateId;

			return this;
		}

		public ITransitionBuilder ToHistory(object stateId)
		{
			if (targetStateId != null)
				throw new InvalidOperationException("Target state already specified for this transition");

			return this;
		}

		public ITransitionBuilder When(Func<bool> guard)
		{
			if (this.guard != null)
				throw new InvalidOperationException("Guard clause already specified for this transition");
			this.guard = guard;

			return this;
		}

		private Transition CreateTransition()
		{
			if (targetStateId == null)
				throw new InvalidOperationException("Cannot create transition without target state");

			if (guard == null)
				return new Transition(Context.Resolve(Id), eventId, Context.Resolve(targetStateId));

			return new GuardedTransition(Context.Resolve(Id), eventId, Context.Resolve(targetStateId), guard);
		}

		private void OnExitingTransitionBuilder()
		{
		}

		#region IStateBuilder forwarders
		public object Id { get { return stateBuilder.Id; } }
		public IStateBuilderContext Context { get { return stateBuilder.Context; } }
		public IEnumerable<IStateBuilder> SubStates { get { return stateBuilder.SubStates; } }
		public IStateBuilder this[object stateId] { get { return stateBuilder[stateId]; } }

		public State Build(State parent)
		{
			throw new InvalidOperationException("Not valid to Build from this context");
		}

		public void AddSecondPassAction(Action<State> action)
		{
			stateBuilder.AddSecondPassAction(action);
		}

		public IStateBuilder AddState(object stateId)
		{
			OnExitingTransitionBuilder();
			return stateBuilder.AddState(stateId);
		}

		public ITransitionBuilder DefaultTransition(object targetState)
		{
			OnExitingTransitionBuilder();
			return stateBuilder.DefaultTransition(targetState);
		}

		public ITransitionBuilder TransitionOn(object eventTarget)
		{
			OnExitingTransitionBuilder();
			return stateBuilder.TransitionOn(eventTarget);
		}

		public ITransitionBuilder TransitionOn(object eventTarget, object targetState)
		{
			OnExitingTransitionBuilder();
			return stateBuilder.TransitionOn(eventTarget, targetState);
		}

		public IStateBuilder InitialState()
		{
			OnExitingTransitionBuilder();
			return stateBuilder.InitialState();
		}
		#endregion

		private class HistoryTargetState
		{
			public object StateId { get; private set; }

		}
	}
}
