namespace Moe.StateMachine.Extensions.Logger
{
	public interface ILogger
	{
		void Log(string message, params object[] messageParams);
	}
}
