using System.Collections.Generic;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestComplexTransitions
	{
		private StateMachineBuilder smb;

		[SetUp]
		public void Setup()
		{
			smb = new StateMachineBuilder();
		}

		[Test]
		public void Test_SuperstateSubState_WithMatchingEvents()
		{
			smb.AddState(States.Green)
				.InitialState()
				.TransitionTo(Events.Change, States.Yellow)
				.AddState(States.GreenChild)
					.InitialState()
					.TransitionTo(Events.Change, States.Red, () => false);
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Green);
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenChild));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_TransitionToSuperState_WithDefaults()
		{
			smb.AddState(States.Green).InitialState();
			smb[States.Green]
				.AddState(States.GreenChild)
				.TransitionTo(Events.Change, States.GreenChild2)
				.InitialState();
			smb[States.Green]
				.AddState(States.GreenChild2)
				.TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
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
			bool flag = false;
			smb.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow, () => flag)
				.TransitionTo(Events.Change, States.Red, () => !flag)
				.InitialState();
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Green);
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
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
			bool flag = false;
			smb.DefaultTransition(States.Green, () => flag);
			smb.DefaultTransition(States.Red, () => !flag);
			smb.AddState(States.Green);
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Green);
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
			sm.Start();

			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_ReentrantEventPosting()
		{
			StateMachine sm = null;
			smb.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionTo(Events.Change, States.Red).OnEnter(s => sm.PostEvent(Events.Change));
			smb.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			sm = new StateMachine("Test", smb);
			sm.Start();

			Assert.IsTrue(sm.InState(States.Green));

			sm.PostEvent(Events.Change);

			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_ReflexiveStateExitEntry()
		{
			smb.AddState(States.GreenParent)
				.InitialState()
				.AddState(States.Green)
					.InitialState();

			smb[States.Green]
				.OnEnter(tr => OnEnter(States.Green))
				.OnExit(tr => OnExit(States.Green))
				.TransitionTo(Events.Change, States.Green);

			StateMachine sm = new StateMachine("Test", smb);
			sm.Start();
			sm.PostEvent(Events.Change);

			Assert.IsTrue(events.Count == 3);
			Assert.AreEqual("Exit: Green", events[1]);
			Assert.AreEqual("Enter: Green", events[2]);
		}

		private List<string> events = new List<string>();
		private void OnEnter(object state)
		{
			events.Add("Enter: " + state.ToString());
		}
		private void OnExit(object state)
		{
			events.Add("Exit: " + state.ToString());
		}
	}
}
