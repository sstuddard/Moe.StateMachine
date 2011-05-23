﻿using System;
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
			GreenChild,
			GreenChild2,
			Red,
			RedChild,
			Yellow,
			YellowChild,
			Gold
		}

		public enum Events
		{
			Change,
			Pulse
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
			sm.PostEvent(Events.Pulse);
			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_TimeoutEvent_TimeoutClearsOnExitState()
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
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
			System.Threading.Thread.Sleep(110);
			sm.PostEvent(Events.Pulse);
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_ConditionalTimeoutEvent_TimeoutClearsAfterFailedEval()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow)
				.Timeout(100, States.Red, () => false)
				.InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			System.Threading.Thread.Sleep(110);
			sm.PostEvent(Events.Pulse);
			Assert.IsTrue(sm.InState(States.Green));
		}

		[Test]
		public void Test_TimeoutEvent_MultipleTimeoutFiresOnSuperstate()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow)
				.Timeout(200, States.Red)
				.InitialState()
					.AddState(States.GreenChild)
					.Timeout(100, States.Yellow, () => false)
					.InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			System.Threading.Thread.Sleep(210);
			sm.PostEvent(Events.Pulse);
			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_TimeoutEvent_TimerAddedAfterLaterTimerInSuperstate()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Red);
			sm.AddState(States.Gold);
			sm.AddState(States.Green)
				.InitialState()
				.Timeout(500, States.Red);
			sm[States.Green][States.GreenChild]
				.InitialState()
				.Timeout(50, States.Red)
				.TransitionTo(Events.Change, States.GreenChild2);
			sm[States.Green][States.GreenChild2]
				.Timeout(200, States.Gold);

			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenChild));
			sm.PostEvent(Events.Change);
			System.Threading.Thread.Sleep(210);
			sm.PostEvent(Events.Pulse);
			Assert.IsTrue(sm.InState(States.Gold));
		}

		[Test]
		public void Test_TimeoutEvent_SubstatesSwappingSuperstateTimesOut()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Red);
			sm.AddState(States.Gold);
			sm.AddState(States.Green)
				.InitialState()
				.Timeout(300, States.Gold);
			sm[States.Green][States.GreenChild]
				.InitialState()
				.Timeout(1000, States.Red)
				.TransitionTo(Events.Change, States.GreenChild2);
			sm[States.Green][States.GreenChild2]
				.Timeout(1000, States.Red)
				.TransitionTo(Events.Change, States.GreenChild);

			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenChild));
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.GreenChild2));
			System.Threading.Thread.Sleep(310);
			sm.PostEvent(Events.Pulse);
			Assert.IsTrue(sm.InState(States.Gold));
		}
	}
}
