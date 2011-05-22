using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jed.StateMachine
{
	/// <summary>
	/// Lazy resolution of states
	/// </summary>
	internal class StateLocator
	{
		private object stateId;
		private StateMachine stateMachine;

		public StateLocator(object id, StateMachine sm)
		{
			this.stateId = id;
			this.stateMachine = sm;
		}

		public static implicit operator State(StateLocator loc)
		{
			State result = loc.stateMachine.GetState(loc.stateId);
			if (result == null)
				throw new InvalidOperationException("Lazy resolution of state " + loc.stateId + " failed.");

			return result;
		}
	}
}
