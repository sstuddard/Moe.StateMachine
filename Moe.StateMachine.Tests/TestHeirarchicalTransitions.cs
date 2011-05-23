using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestHeirarchicalTransitions
	{
		private List<string> events;

		public enum States
		{
			GreenParent,
			GreenChild,
			GreenGrandChild,
			RedParent,
			RedChild,
			Yellow
		}

		public enum Events
		{
			Panic,
			Change
		}

		[SetUp]
		public void Setup()
		{
			events = new List<string>();
		}

		[Test]
		public void Test_DefaultTransition_Multiple_Entry()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.GreenParent).InitialState()
				.AddState(States.GreenChild).InitialState();
			sm.AddState(States.RedParent);

			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenParent));
			Assert.IsTrue(sm.InState(States.GreenChild));
		}

		[Test]
		public void Test_Transition_ToSuperstateWithDefault()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.GreenParent)
				.AddState(States.GreenChild).InitialState();
			sm.AddState(States.RedParent).TransitionTo(Events.Change, States.GreenParent).InitialState();

			sm.Start();

			Assert.IsTrue(sm.InState(States.RedParent));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.GreenChild));
		}

		[Test]
		public void Test_Transition_ThreeLevelToTwoLevelTransition()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.GreenParent).OnEnter(OnEnter).OnExit(OnExit).InitialState()
				.AddState(States.GreenChild).OnEnter(OnEnter).OnExit(OnExit).InitialState()
					.AddState(States.GreenGrandChild).InitialState()
						.TransitionTo(Events.Change, States.RedChild).OnEnter(OnEnter).OnExit(OnExit);
			sm.AddState(States.RedParent).OnEnter(OnEnter).OnExit(OnExit)
				.AddState(States.RedChild).OnEnter(OnEnter).OnExit(OnExit);

			sm.Start();

			events.Clear();

			Assert.IsTrue(sm.InState(States.GreenParent));
			Assert.IsTrue(sm.InState(States.GreenChild));
			Assert.IsTrue(sm.InState(States.GreenGrandChild));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.RedParent));
			Assert.IsTrue(sm.InState(States.RedChild));

			Assert.AreEqual("Exit: GreenGrandChild", events[0]);
			Assert.AreEqual("Exit: GreenChild", events[1]);
			Assert.AreEqual("Exit: GreenParent", events[2]);
			Assert.AreEqual("Enter: RedParent", events[3]);
			Assert.AreEqual("Enter: RedChild", events[4]);
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
