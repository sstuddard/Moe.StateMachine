using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class TransitionDirector
	{
		private State state;
		private List<Transition> transitions;

		public TransitionDirector(State state)
		{
			this.state = state;
			this.transitions = new List<Transition>();
		}

		public TransitionInstance AcceptTransition(object eventTarget)
		{
			TransitionInstance result = null;
			foreach (Transition transition in transitions)
			{
				if (transition.Matches(eventTarget))
				{
					if (result != null)
						throw new InvalidOperationException("Multiple states eligible for transition");
					result = new TransitionInstance(state, transition);
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
