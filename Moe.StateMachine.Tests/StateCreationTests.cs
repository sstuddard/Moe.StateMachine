using System;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
    public class TestStateCreation
	{
		private StateMachineBuilder smb;

		[SetUp]
		public void Setup()
		{
			smb = new StateMachineBuilder();
		}

		[Test]
        public void Test_Create_SimpleMachine()
        {
        	smb.AddState(States.Green);

			Assert.IsNotNull(smb[States.Green]);
        }

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_Create_StateUniqueness()
		{
			smb.AddState(States.Green);
			smb.AddState(States.Green);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_Create_StateUniqueness_EntireStateMachine()
		{
			smb.AddState(States.Green).AddState(States.Red);
			smb.AddState(States.Red);
		}

		[Test]
		public void Test_Create_TwoStates()
		{
			smb.AddState(States.Green);
			smb.AddState(States.Red);

			Assert.IsNotNull(smb[States.Green]);
			Assert.IsNotNull(smb[States.Red]);
		}

		[Test]
		public void Test_Create_TwoStates_WithTransition()
		{
			smb.AddState(States.Green);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			Assert.IsNotNull(smb[States.Green]);
			Assert.IsNotNull(smb[States.Red]);
		}

		[Test]
		public void Test_Create_CanCreateTransitionBeforeStateDefined()
		{
			smb.AddState(States.Green).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red);

			Assert.IsNotNull(smb[States.Green]);
			Assert.IsNotNull(smb[States.Red]);
		}

		[Test]
		public void Test_Create_InitialState()
		{
			smb.AddState(States.Green);
			smb.AddState(States.Red);

			smb.DefaultTransition(States.Red);
		}

		[Test]
		public void Test_Create_Heirarchy_ParentChild()
		{
			smb.AddState(States.Green)
				.AddState(States.GreenChild);

			smb.AddState(States.Red);

			smb.DefaultTransition(States.Red);
		}

		[Test]
		public void Test_State_ContainsState()
		{
			smb.AddState(States.Green)
				.AddState(States.GreenChild);

			smb.AddState(States.Red);

			Assert.AreEqual(smb[States.GreenChild].Id, States.GreenChild);
		}

		[Test]
		public void Test_State_CreateDeepByIndexers()
		{
			smb[States.Green][States.GreenChild][States.Red].InitialState();
			Assert.IsNotNull(smb[States.Green][States.GreenChild][States.Red].Id);
		}

		[Test]
		public void Test_ShortCircuitIndexer_CreatesState()
		{
			Assert.IsNotNull(smb[States.Green]);
			Assert.IsNotNull(smb[States.Green][States.GreenChild]);
			Assert.IsNotNull(smb[States.Yellow]);

			Assert.IsNotNull(smb[States.GreenChild]);
		}
	}
}
