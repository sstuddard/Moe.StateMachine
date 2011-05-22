using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class TransitionInstance
	{
		private Transition transition;
		private State sourceState;

		public TransitionInstance(State sourceState, Transition transition)
		{
			this.sourceState = sourceState;
			this.transition = transition;
		}

		public State Transition()
		{
			// Exit path
			State topMost = Exit(sourceState, transition.TargetState);
			Enter(topMost, transition.TargetState);

			return transition.TargetState;
		}

		private State Exit(State exiting, State target)
		{
			// Special case for root
			if (exiting.Parent == null)
				return exiting;

			exiting.Exit();
			State parent = exiting.Parent;

			if (parent.ContainsState(target))
				return parent;
			else
				return Exit(parent, target);
		}

		private void Enter(State entering, State target)
		{
			entering.Enter();

			foreach (State subState in entering.Substates)
			{
				if (subState.ContainsState(target.Id))
					Enter(subState, target);
			}
		}
	}
}
