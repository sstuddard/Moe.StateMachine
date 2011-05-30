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

		public ILogger Logger { get { return logger; } }

		public void Initialize(StateMachine sm)
		{
			stateMachine = sm;
			stateMachine.Starting += delegate
			                         	{
											logger.Log("Starting state machine");
											if (root == null)
											{
												root = stateMachine.RootNode;
												root.VisitChildren(AttachToState);
											}
			                         	};
			stateMachine.EventPosted += (s, e) => logger.Log("Event posted: {0}", e.Event.Event);
			stateMachine.EventProcessed += (s, e) => logger.Log("Event processed: {0}", e.Event.Event);
		}

		private void AttachToState(State state)
		{
			state.Entered += delegate { logger.Log("Entered {0}", state); };
			state.Exited += delegate { logger.Log("Exited {0}", state); };
		}
	}
}
