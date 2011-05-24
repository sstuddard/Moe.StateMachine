using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Actions
{
	public class ContextAction<T> : IActionPerformer
	{
		public Action<object, T> Action { get; private set; }

		public ContextAction(Action<object, T> action)
		{
			Action = action;
		}

		public virtual void Perform(object stateId, object context)
		{
			Action(stateId, (T)context);
		}
	}
}
