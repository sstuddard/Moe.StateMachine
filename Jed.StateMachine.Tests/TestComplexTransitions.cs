using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Jed.StateMachine.Tests
{
	[TestFixture]
	public class TestComplexTransitions
	{
		public enum States
		{
			Green,
			Red,
			Yellow
		}

		public enum Events
		{
			Change
		}

		[Test]
		public void Test_ReentrantEventPosting()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red).OnEnter(s => sm.PostEvent(Events.Change));
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));

			sm.PostEvent(Events.Change);

			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_TimeoutEvent_TimeoutFires()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow)
				.Timeout(100, States.Red)
				.InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			System.Threading.Thread.Sleep(110);
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_TimeoutEvent_TimeoutClearsOnExitState()
		{
			
		}

		[Test]
		public void Test_TimeoutEvent_TimeoutFiresOnSuperstate()
		{

		}
	}
}
