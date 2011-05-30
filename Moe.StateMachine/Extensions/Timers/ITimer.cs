using System;

namespace Moe.StateMachine.Extensions.Timers
{
	public interface ITimer
	{
		event EventHandler<EventArgs> ActiveChanged;

		bool Active { get; }
		DateTime NextTime { get; }

		void Execute();
	}
}
