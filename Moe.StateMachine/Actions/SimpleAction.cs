using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Actions
{
	public class SimpleAction : IActionPerformer
	{
		public Action<object> Action { get; private set; }

		public SimpleAction(Action<object> action)
		{
			Action = action;
		}

		public virtual void Perform(object stateId, object context)
		{
			Action(stateId);
		}
	}
}
