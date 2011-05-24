using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestComplexTransitions
	{
		[Test]
		public void Test_SuperstateSubState_WithMatchingEvents()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.InitialState()
				.TransitionTo(Events.Change, States.Yellow)
				.AddState(States.GreenChild)
					.InitialState()
					.TransitionTo(Events.Change, States.Red, () => false);
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Green);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenChild));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_TransitionToSuperState_WithDefaults()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).InitialState();
			sm[States.Green]
				.AddState(States.GreenChild)
				.TransitionTo(Events.Change, States.GreenChild2)
				.InitialState();
			sm[States.Green]
				.AddState(States.GreenChild2)
				.TransitionTo(Events.Change, States.Green);

			sm.Start();
			Assert.IsTrue(sm.InState(States.GreenChild));
			Assert.IsTrue(sm.InState(States.Green));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.GreenChild2));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.GreenChild));
			Assert.IsTrue(sm.InState(States.Green));
		}

		[Test]
		public void Test_MultipleGuardedTransitions()
		{
			StateMachine sm = new StateMachine();
			bool flag = false;
			sm.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow, () => flag)
				.TransitionTo(Events.Change, States.Red, () => !flag)
				.InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Green);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Red));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Green));
			flag = true;
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_MultipleGuardedDefaultTransitions()
		{
			StateMachine sm = new StateMachine();
			bool flag = false;
			sm.DefaultTransition(States.Green, () => flag);
			sm.DefaultTransition(States.Red, () => !flag);
			sm.AddState(States.Green);
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Green);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm.Start();

			Assert.IsTrue(sm.InState(States.Red));
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
		public void Test_ReflexiveStateExitEntry()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.GreenParent)
				.InitialState()
				.AddState(States.Green)
					.InitialState();

			sm[States.Green]
				.OnEnter(OnEnterState)
				.OnExit(OnExitState)
				.TransitionTo(Events.Change, States.Green);

			sm.Start();
			sm.PostEvent(Events.Change);

			Assert.IsTrue(events.Count == 3);
			Assert.AreEqual("Exit: Green", events[1]);
			Assert.AreEqual("Enter: Green", events[2]);
		}

		private List<string> events = new List<string>();
		private void OnEnterState(object stateId)
		{
			events.Add("Enter: " + stateId.ToString());
		}
		private void OnExitState(object stateId)
		{
			events.Add("Exit: " + stateId.ToString());
		}
	}
}
