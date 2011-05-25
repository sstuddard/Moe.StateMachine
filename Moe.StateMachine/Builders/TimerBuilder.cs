using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.States;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine.Builders
{
	public class TimerBuilder : StateBuilderSkeleton
	{
		public TimerBuilder(IStateBuilder source)
			: base(source)
		{
		}

		public override IStateBuilder Timeout(int timeoutInMilliseconds, object targetState)
		{
			Transition timeout = new Transition(State, StateMachine.TimeoutEvent, new StateLocator(targetState, StateMachine));
			return Timeout(timeoutInMilliseconds, timeout);
		}

		public override IStateBuilder Timeout(int timeoutInMilliseconds, object targetState, Func<bool> guard)
		{
			Transition timeout =
				new GuardedTransition(State, StateMachine.TimeoutEvent, new StateLocator(targetState, StateMachine), guard);
			return Timeout(timeoutInMilliseconds, timeout);
		}

		private IStateBuilder Timeout(int timeoutInMilliseconds, Transition transition)
		{
			State.AddTransition(transition);
			OnEnter(s => StateMachine.RegisterTimer(State, DateTime.Now.AddMilliseconds(timeoutInMilliseconds)));
			OnExit(s => StateMachine.RemoveTimer(State));
			return sourceBuilder;
		}
	}
}
