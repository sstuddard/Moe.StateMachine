using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Actions;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Builders
{
	/// <summary>
	/// State builder skeleton for decoration of a StateBuilder.  By default,
	/// VerifyExitingBuilder() will be called prior to delegation to the source
	/// builder so the decorating builder can ensure all information has been
	/// provided.
	/// </summary>
	public class StateBuilderSkeleton : IStateBuilder
	{
		protected IStateBuilder sourceBuilder;

		public StateBuilderSkeleton(IStateBuilder source)
		{
			this.sourceBuilder = source;
		}

		public virtual void VerifyExitingBuilder()
		{
			// Override with exceptions to validate exiting this builder
		}

		public object Id
		{
			get { return sourceBuilder.Id; }
		}

		public State State
		{
			get { return sourceBuilder.State; }
		}

		public StateMachine StateMachine
		{
			get { return sourceBuilder.StateMachine; }
		}

		public IStateBuilder this[object idx]
		{
			get { return sourceBuilder[idx]; }
		}

		public virtual IStateBuilder AddState(object newStateId)
		{
			VerifyExitingBuilder();
			return sourceBuilder.AddState(newStateId);
		}

		public virtual IStateBuilder DefaultTransition(object targetState)
		{
			VerifyExitingBuilder();
			return sourceBuilder.DefaultTransition(targetState);
		}

		public virtual IStateBuilder DefaultTransition(object targetState, Func<bool> guard)
		{
			VerifyExitingBuilder();
			return sourceBuilder.DefaultTransition(targetState, guard);
		}

		public virtual IStateBuilder TransitionTo(object eventTarget, object targetState)
		{
			VerifyExitingBuilder();
			return sourceBuilder.TransitionTo(eventTarget, targetState);
		}

		public virtual IStateBuilder TransitionTo(object eventTarget, object targetState, Func<bool> guard)
		{
			VerifyExitingBuilder();
			return sourceBuilder.TransitionTo(eventTarget, targetState, guard);
		}

		public virtual IStateBuilder InitialState()
		{
			VerifyExitingBuilder();
			return sourceBuilder.InitialState();
		}

		public virtual IStateBuilder Timeout(int timeoutInMilliseconds, object targetState)
		{
			VerifyExitingBuilder();
			return sourceBuilder.Timeout(timeoutInMilliseconds, targetState);
		}

		public virtual IStateBuilder Timeout(int timeoutInMilliseconds, object targetState, Func<bool> guard)
		{
			VerifyExitingBuilder();
			return sourceBuilder.Timeout(timeoutInMilliseconds, targetState, guard);
		}

		public virtual IStateBuilder OnEnter(Action<TransitionReceipt> action)
		{
			VerifyExitingBuilder();
			return sourceBuilder.OnEnter(action);
		}

		public virtual IStateBuilder OnExit(Action<TransitionReceipt> action)
		{
			VerifyExitingBuilder();
			return sourceBuilder.OnExit(action);
		}
	}
}
