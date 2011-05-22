using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Jed.StateMachine;

namespace Jed.StateMachine.Tests
{
	[TestFixture]
	public class TestSimpleTransitions
	{
		private List<string> events;

		public enum States
		{
			Green,
			Red,
			Yellow,
			GreenChild
		}

		public enum Events
		{
			WalkButtonPushed,
			Change
		}

		[SetUp]
		public void Setup()
		{
			events = new List<string>();
		}

		[Test]
		public void Test_Transitions_DefaultTransition()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
		}

		[Test]
		public void Test_Transitions_BasicTransitions()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));

			sm.PostEvent(Events.Change);

			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_Transitions_BasicTransitions_EnterActions()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).OnEnter(OnEnter).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).OnEnter(OnEnter).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).OnEnter(OnEnter).TransitionTo(Events.Change, States.Green);

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
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).OnExit(OnExit).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).OnExit(OnExit).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).OnExit(OnExit).TransitionTo(Events.Change, States.Green);

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

		private void OnEnter(object stateEntered)
		{
			events.Add("Enter: " + stateEntered.ToString());
		}

		private void OnExit(object stateEntered)
		{
			events.Add("Exit: " + stateEntered.ToString());
		}
	}
}
