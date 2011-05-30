using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Extensions.Timers
{
	public class TimeoutPlugin : IPlugIn
	{
		private StateMachine stateMachine;

		public void Initialize(StateMachine sm)
		{
			stateMachine = sm;
		}

		public void PostTimeout(State state)
		{
			stateMachine.PostEvent(new SingleStateEventInstance(state, TimerBuilder.TimeoutEvent));
		}
	}
}
