using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Moe.StateMachine;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
    public class TestStateCreation
    {
		public enum States
		{
			Green,
			Red,
			GreenChild
		}

		public enum Events
		{
			Change
		}

		[Test]
        public void Test_Create_SimpleMachine()
        {
			StateMachine sm = new StateMachine();
        	sm.AddState(States.Green);

			Assert.IsNotNull(sm[States.Green]);
        }

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_Create_StateUniqueness()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green);
			sm.AddState(States.Green);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_Create_StateUniqueness_EntireStateMachine()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).AddState(States.Red);
			sm.AddState(States.Red);
		}

		[Test]
		public void Test_Create_TwoStates()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green);
			sm.AddState(States.Red);

			Assert.IsNotNull(sm[States.Green]);
			Assert.IsNotNull(sm[States.Red]);
		}

		[Test]
		public void Test_Create_TwoStates_WithTransition()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

			Assert.IsNotNull(sm[States.Green]);
			Assert.IsNotNull(sm[States.Red]);
		}

		[Test]
		public void Test_Create_CanCreateTransitionBeforeStateDefined()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red);

			Assert.IsNotNull(sm[States.Green]);
			Assert.IsNotNull(sm[States.Red]);
		}

		[Test]
		public void Test_Create_InitialState()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green);
			sm.AddState(States.Red);

			sm.DefaultTransition(States.Red);
		}

		[Test]
		public void Test_Create_Heirarchy_ParentChild()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.AddState(States.GreenChild);

			sm.AddState(States.Red);

			sm.DefaultTransition(States.Red);
		}

		[Test]
		public void Test_State_ContainsState()
		{
			StateMachine sm = new StateMachine();
			sm.AddState(States.Green)
				.AddState(States.GreenChild);

			sm.AddState(States.Red);

			Assert.AreEqual(sm[States.GreenChild].Id, States.GreenChild);
		}

		[Test]
		public void Test_State_CreateDeepByIndexers()
		{
			StateMachine sm = new StateMachine();
			sm[States.Green][States.GreenChild][States.Red].InitialState();
			Assert.IsNotNull(sm[States.Green][States.GreenChild][States.Red].Id);
		}
	}
}
