using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class TransitionDirector
	{
		private List<Transition> transitions;

		public TransitionDirector()
		{
			transitions = new List<Transition>();
		}

		public Transition FindTransition(object eventTarget)
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
