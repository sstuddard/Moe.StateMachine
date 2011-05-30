using Moe.StateMachine.Extensions;

namespace Moe.StateMachine.Events
{
	public class SynchronousEventProcessor : EventProcessor, IPlugIn
	{
		private StateMachine stateMachine;

		public void Initialize(StateMachine sm)
		{
			stateMachine = sm;
		}

		public override void AddEvent(EventInstance eventToAdd)
		{
			base.AddEvent(eventToAdd);

			while (CanProcess)
				ProcessNextEvent(stateMachine.CurrentState);
		}
	}
}
