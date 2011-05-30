namespace Moe.StateMachine.Extensions.Logger
{
	public static class LoggerBuilder
	{
		public static StateMachine Logger(this StateMachine sm, ILogger logger)
		{
			LoggerPlugIn plugin = new LoggerPlugIn(logger);
			sm.AddPlugIn(plugin);

			return sm;
		}
	}
}
