using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Actions
{
	public class SimpleAction : IActionPerformer
	{
		public ActionType Type { get; private set; }
		public Action<TransitionReceipt> Action { get; private set; }

		public SimpleAction(ActionType type, Action<TransitionReceipt> action)
		{
			Type = type;
			Action = action;
		}

		public virtual void Perform(TransitionReceipt receipt)
		{
			Action(receipt);
		}
	}
}
