using System;
using System.Collections.Generic;
using Moe.StateMachine.Extensions;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestSimpleTransitions : BaseTest
	{
		private List<string> events;

		[SetUp]
		public void Setup()
		{
			events = new List<string>();
		}

		[Test]
		public void Test_Transitions_DefaultTransition()
		{
			smb.AddState(States.Green).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
		}

		[Test]
		public void Test_Transitions_BasicTransitions()
		{
			smb.AddState(States.Green).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));

			sm.PostEvent(Events.Change);

			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_Transitions_MultiTransitionFromState()
		{
			smb.AddState(States.Green)
				.TransitionOn(Events.Change, States.Yellow)
				.TransitionOn(Events.Panic, States.Red)
				.InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Red));
			sm.PostEvent(Events.Panic);
			Assert.IsTrue(sm.InState(States.Red));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Green));
			sm.PostEvent(Events.Panic);
			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_Transitions_BasicTransitions_EnterActions()
		{
			smb.AddState(States.Green).OnEnter(tr => CaptureState("Enter", States.Green)).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).OnEnter(tr => CaptureState("Enter", States.Yellow)).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).OnEnter(tr => CaptureState("Enter", States.Red)).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);

			Assert.IsTrue(events[0].Contains("Green"));
			Assert.IsTrue(events[1].Contains("Yellow"));
			Assert.IsTrue(events[2].Contains("Red"));
			Assert.IsTrue(events[3].Contains("Green"));
			Assert.IsTrue(events[4].Contains("Yellow"));
		}

		[Test]
		public void Test_Transitions_BasicTransitions_ExitActions()
		{
			smb.AddState(States.Green).OnExit(tr => CaptureState("Exit", States.Green)).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).OnExit(tr => CaptureState("Exit", States.Yellow)).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).OnExit(tr => CaptureState("Exit", States.Red)).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);
			sm.PostEvent(Events.Change);

			Assert.IsTrue(events[0].Contains("Green"));
			Assert.IsTrue(events[1].Contains("Yellow"));
			Assert.IsTrue(events[2].Contains("Red"));
			Assert.IsTrue(events[3].Contains("Green"));
		}

		[Test]
		public void Test_Transitions_BasicTransitions_Conditional()
		{
			bool allow = false;
			smb.AddState(States.Green)
				.TransitionOn(Events.Change, States.Yellow).When(() => allow)
				.InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			// Transition should not go
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Green));

			allow = true;
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		private void CaptureState(string prefix, object state)
		{
			events.Add(String.Format("{0}: {1}", prefix, state.ToString()));
		}
	}
}
