using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Jed.StateMachine.Tests
{
	[TestFixture]
	public class TestHeirarchicalTransitions
	{
		private List<string> events;

		public enum States
		{
			GreenParent,
			GreenChild,
			RedParent,
			RedChild,
			Yellow
		}

		public enum Events
		{
			Panic,
			Change
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
	}
}
