using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moe.StateMachine;

namespace Moe.StateMachine.Tests
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
			PanicButton,
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
		public void Test_Transitions_MultiTransitionFromState()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow)
				.TransitionTo(Events.PanicButton, States.Red)
				.InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Red));
			sm.PostEvent(Events.PanicButton);
			Assert.IsTrue(sm.InState(States.Red));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Green));
			sm.PostEvent(Events.PanicButton);
			Assert.IsTrue(sm.InState(States.Red));
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

		[Test]
		public void Test_Transitions_BasicTransitions_Conditional()
		{
			StateMachine sm = new StateMachine();
			bool allow = false;
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow, () => { return allow; }).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			// Transition should not go
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Green));

			allow = true;
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_Transitions_EntryExit_WithContext()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow)
				.OnExit<Dictionary<string,string>>(OnEnterExitWithContext)
				.InitialState();
			sm.AddState(States.Yellow)
				.TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red)
				.OnEnter<Dictionary<string, string>>(OnEnterExitWithContext)
				.TransitionTo(Events.Change, States.Green);

			Dictionary<string, string> context = new Dictionary<string, string>();
			context["Did"] = "exit";

			sm.Start();
			sm.PostEvent(Events.Change, context);
			Assert.IsNotNull(postedContext);
			Assert.IsTrue(postedContext.ContainsKey("Did"));
			Assert.AreEqual("exit", postedContext["Did"]);

			postedContext = null;
			context["Did"] = "enter";
			sm.PostEvent(Events.Change, context);
			Assert.IsNotNull(postedContext);
			Assert.IsTrue(postedContext.ContainsKey("Did"));
			Assert.AreEqual("enter", postedContext["Did"]);
		}

		private Dictionary<string, string> postedContext;
		private void OnEnterExitWithContext(object stateEntered, Dictionary<string, string> context)
		{
			postedContext = context;
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
