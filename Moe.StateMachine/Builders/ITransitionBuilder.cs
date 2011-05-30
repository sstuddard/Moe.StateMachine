using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Builders
{
	public interface ITransitionBuilder : IStateBuilder
	{
		ITransitionBuilder To(object stateId);
		ITransitionBuilder ToHistory(object stateId);
		ITransitionBuilder When(Func<bool> guard);
	}
}
