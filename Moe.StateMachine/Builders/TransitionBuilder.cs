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
			targetStateId = new HistoryStateId(stateId);
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
				throw new InvalidOperationException("Cannot create " + stateBuilder.Id + ":" + eventId + " transition without target state");

			if (guard == null)
				return new Transition(Context.Resolve(Id), eventId, Context.Resolve(targetStateId));

			return new GuardedTransition(Context.Resolve(Id), eventId, Context.Resolve(targetStateId), guard);
		}

		#region IStateBuilder forwarders
		public object Id { get { return stateBuilder.Id; } }
		public IStateBuilderContext Context { get { return stateBuilder.Context; } }
		public IEnumerable<IBaseStateBuilder> SubStates { get { return stateBuilder.SubStates; } }
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
			return stateBuilder.AddState(stateId);
		}

		public IStateBuilder AddHistory()
		{
			return stateBuilder.AddHistory();
		}

		public IStateBuilder AddDeepHistory()
		{
			return stateBuilder.AddDeepHistory();
		}

		public ITransitionBuilder DefaultTransition(object targetState)
		{
			return stateBuilder.DefaultTransition(targetState);
		}

		public ITransitionBuilder TransitionOn(object eventTarget)
		{
			return stateBuilder.TransitionOn(eventTarget);
		}

		public ITransitionBuilder TransitionOn(object eventTarget, object targetState)
		{
			return stateBuilder.TransitionOn(eventTarget, targetState);
		}

		public IStateBuilder InitialState()
		{
			return stateBuilder.InitialState();
		}
		#endregion
	}
}
