﻿using System.Threading;
using Moe.StateMachine.Extensions.Asynchronous;
using Moe.StateMachine.Extensions.Logger;
using Moe.StateMachine.Extensions.Timers;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TimeoutTests : BaseTest
	{
		[Test]
		public void Test_TimeoutEvent_TimeoutFires()
		{
			smb.AddState(States.Green)
				.TransitionOn(Events.Change, States.Yellow)
				.Timeout(100, States.Red)
				.InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine().Asynchronous();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			System.Threading.Thread.Sleep(110);
			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_TimeoutEvent_TimeoutClearsOnExitState()
		{
			smb.AddState(States.Green)
				.TransitionOn(Events.Change, States.Yellow)
				.Timeout(100, States.Red)
				.InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
			System.Threading.Thread.Sleep(120);
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_ConditionalTimeoutEvent_TimeoutClearsAfterFailedEval()
		{
			smb.AddState(States.Green)
				.TransitionOn(Events.Change, States.Yellow)
				.Timeout(100, States.Red, () => false)
				.InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine().Asynchronous();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			System.Threading.Thread.Sleep(110);
			Assert.IsTrue(sm.InState(States.Green));
		}

		[Test]
		public void Test_TimeoutEvent_MultipleTimeoutFiresOnSuperstate()
		{
			smb.AddState(States.Green)
				.TransitionOn(Events.Change, States.Yellow)
				.Timeout(200, States.Red)
				.InitialState()
					.AddState(States.GreenChild)
					.Timeout(100, States.Yellow, () => false)
					.InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine().Asynchronous();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			System.Threading.Thread.Sleep(210);
			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_TimeoutEvent_TimerAddedAfterLaterTimerInSuperstate()
		{
			smb.AddState(States.Red);
			smb.AddState(States.Gold);
			smb.AddState(States.Green)
				.InitialState()
				.Timeout(500, States.Red);
			smb[States.Green][States.GreenChild]
				.InitialState()
				.Timeout(50, States.Red)
				.TransitionOn(Events.Change, States.GreenChild2);
			smb[States.Green][States.GreenChild2]
				.Timeout(200, States.Gold);

			CreateStateMachine().Asynchronous();
			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenChild));
			sm.PostEvent(Events.Change);
			System.Threading.Thread.Sleep(210);
			Assert.IsTrue(sm.InState(States.Gold));
		}

		[Test]
		public void Test_TimeoutEvent_SubstatesSwappingSuperstateTimesOut()
		{
			smb.AddState(States.Red);
			smb.AddState(States.Gold);
			smb.AddState(States.Green)
				.InitialState()
				.Timeout(300, States.Gold);
			smb[States.Green][States.GreenChild]
				.InitialState()
				.Timeout(1000, States.Red)
				.TransitionOn(Events.Change, States.GreenChild2);
			smb[States.Green][States.GreenChild2]
				.Timeout(1000, States.Red)
				.TransitionOn(Events.Change, States.GreenChild);

			CreateStateMachine();
			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenChild));
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.GreenChild2));
			System.Threading.Thread.Sleep(310);
			Assert.IsTrue(sm.InState(States.Gold));
		}
	}
}
