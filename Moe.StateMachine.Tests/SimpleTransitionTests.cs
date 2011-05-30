using System;
using System.Collections.Generic;
using Moe.StateMachine.Extensions;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestSimpleTransitions
	{
		private List<string> events;
		private StateMachineBuilder smb;

		[SetUp]
		public void Setup()
		{
			events = new List<string>();
			smb = new StateMachineBuilder();
		}

		[Test]
		public void Test_Transitions_DefaultTransition()
		{
			smb.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
		}

		[Test]
		public void Test_Transitions_BasicTransitions()
		{
			smb.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));

			sm.PostEvent(Events.Change);

			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_Transitions_MultiTransitionFromState()
		{
			smb.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow)
				.TransitionTo(Events.Panic, States.Red)
				.InitialState();
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
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
			smb.AddState(States.Green).OnEnter(tr => CaptureState("Enter", States.Green)).TransitionTo(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).OnEnter(tr => CaptureState("Enter", States.Yellow)).TransitionTo(Events.Change, States.Red);
			smb.AddState(States.Red).OnEnter(tr => CaptureState("Enter", States.Red)).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
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
			smb.AddState(States.Green).OnExit(tr => CaptureState("Exit", States.Green)).TransitionTo(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).OnExit(tr => CaptureState("Exit", States.Yellow)).TransitionTo(Events.Change, States.Red);
			smb.AddState(States.Red).OnExit(tr => CaptureState("Exit", States.Red)).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
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
			smb.AddState(States.Green).TransitionTo(Events.Change, States.Yellow, () => { return allow; }).InitialState();
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
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
