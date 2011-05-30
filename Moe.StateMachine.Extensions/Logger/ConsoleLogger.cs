using System;

namespace Moe.StateMachine.Extensions.Logger
{
	public class ConsoleLogger : ILogger
	{
		public void Log(string message, params object[] messageParams)
		{
			Console.WriteLine(message, messageParams);
		}
	}
}
