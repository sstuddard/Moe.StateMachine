using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	internal class TimerManager
	{
		private Dictionary<State, DateTime> timers;

		public TimerManager()
		{
			timers = new Dictionary<State, DateTime>();
		}

		public void SetTimer(State state, DateTime timeout)
		{
			timers[state] = timeout;
		}

		public void ClearTimer(State state)
		{
			if (timers.ContainsKey(state))
				timers.Remove(state);
		}

		public State GetNextStateTimeout()
		{
			State earliest = null;
			foreach (State state in timers.Keys)
			{
				if (earliest == null)
					earliest = state;
				else
				{
					if (timers[state] < timers[earliest])
						earliest = state;
				}
			}

			return earliest;
		}
	}
}
