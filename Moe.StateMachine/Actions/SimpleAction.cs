using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Actions
{
	public class SimpleAction : IActionPerformer
	{
		public Action<TransitionReceipt> Action { get; private set; }

		public SimpleAction(Action<TransitionReceipt> action)
		{
			Action = action;
		}

		public virtual void Perform(TransitionReceipt receipt)
		{
			Action(receipt);
		}
	}
}
