using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Extensions.Logger;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	public class BaseTest
	{
		protected StateMachineBuilder smb;
		protected StateMachine sm;

		[SetUp]
		public void BaseSetup()
		{
			smb = new StateMachineBuilder();
		}

		protected StateMachine CreateStateMachine()
		{
			sm = new StateMachine("Test", smb).Logger(new ConsoleLogger());
			return sm;
		}
	}
}
