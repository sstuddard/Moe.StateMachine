using Moe.StateMachine.States;

namespace Moe.StateMachine.Extensions.Logger
{
	public class LoggerPlugIn : IPlugIn
	{
		private StateMachine stateMachine;
		private State root;
		private ILogger logger;

		public LoggerPlugIn(ILogger logger)
		{
			this.logger = logger;
		}

		public void Initialize(StateMachine sm)
		{
			stateMachine = sm;
			stateMachine.Starting += delegate
			                         	{
											Log("Starting state machine");
											if (root == null)
											{
												root = stateMachine.RootNode;
												root.VisitChildren(AttachToState);
											}
			                         	};
			stateMachine.EventPosted += (s, e) => Log("Event posted: {0}", e.Event.Event);
			stateMachine.EventProcessed += (s, e) => Log("Event processed: {0}", e.Event.Event);
		}

		private void AttachToState(State state)
		{
			state.Entered += delegate { Log("Entered {0}", state); };
			state.Exited += delegate { Log("Exited {0}", state); };
		}

		public void Log(string message, params object[] messageParams)
		{
			if (logger != null)
				logger.Log("[fsm:{0}] {1}", stateMachine.Name, string.Format(message, messageParams));
		}
	}
}
