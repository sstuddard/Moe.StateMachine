using System;
using System.Collections.Generic;
using Moe.StateMachine.Events;
using Moe.StateMachine.Transitions;

namespace Moe.StateMachine
{
	public class TransitionDirector
	{
		private List<Transition> transitions;

		public TransitionDirector()
		{
			this.transitions = new List<Transition>();
		}

		public Transition MatchTransition(EventInstance eventTarget)
		{
			Transition result = null;
			foreach (Transition transition in transitions)
			{
				if (transition.Matches(eventTarget))
				{
					if (result != null)
						throw new InvalidOperationException("Multiple states eligible for transition");
					result = transition;
				}
			}
			return result;
		}

		public void AddTransition(Transition transition)
		{
			transitions.Add(transition);
		}
	}
}
