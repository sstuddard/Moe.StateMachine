using System.Collections.Generic;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestHeirarchicalTransitions
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
		public void Test_DefaultTransition_Multiple_Entry()
		{
			smb.AddState(States.GreenParent).InitialState()
				.AddState(States.GreenChild).InitialState();
			smb.AddState(States.RedParent);

			StateMachine sm = new StateMachine("Test", smb);
			sm.Start();

			Assert.IsTrue(sm.InState(States.GreenParent));
			Assert.IsTrue(sm.InState(States.GreenChild));
		}

		[Test]
		public void Test_Transition_ToSuperstateWithDefault()
		{
			smb.AddState(States.GreenParent)
				.AddState(States.GreenChild).InitialState();
			smb.AddState(States.RedParent).TransitionOn(Events.Change, States.GreenParent).InitialState();

			StateMachine sm = new StateMachine("Test", smb);
			sm.Start();

			Assert.IsTrue(sm.InState(States.RedParent));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.GreenChild));
		}

		[Test]
		public void Test_Transition_ThreeLevelToTwoLevelTransition()
		{
			smb.AddState(States.GreenParent)
				.OnEnter(tr => OnEnter(States.GreenParent))
				.OnExit(tr => OnExit(States.GreenParent))
				.InitialState()
				.AddState(States.GreenChild)
					.OnEnter(tr => OnEnter(States.GreenChild))
					.OnExit(tr => OnExit(States.GreenChild))
					.InitialState()
						.AddState(States.GreenGrandChild)
							.InitialState()
							.TransitionOn(Events.Change, States.RedChild)
							.OnEnter(tr => OnEnter(States.GreenGrandChild))
							.OnExit(tr => OnExit(States.GreenGrandChild));
			smb.AddState(States.RedParent)
				.OnEnter(tr => OnEnter(States.RedParent))
				.OnExit(tr => OnExit(States.RedParent))
				.AddState(States.RedChild)
					.OnEnter(tr => OnEnter(States.RedChild))
					.OnExit(tr => OnExit(States.RedChild));

			StateMachine sm = new StateMachine("Test", smb);
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
