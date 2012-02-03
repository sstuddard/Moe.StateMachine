using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.Extensions.StateDelegate;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class StateDelegateTests : BaseTest
	{
		public const string StateA = "StateA";
		public const string StateB = "StateB";
		public const string StateC = "StateC";
		public const string StateD = "StateD";

		private string lastDelegateResult;
		private DelegateObject target;

		[SetUp]
		public void Setup()
		{
			lastDelegateResult = null;
			smb.AddState(StateA);
			smb.DefaultTransition(StateA);
			smb[StateA].AddState(StateB);
			smb[StateA].DefaultTransition(StateB);
			smb.AddState(StateC);
			smb[StateB].TransitionOn("event").To(StateC);
			smb[StateC].TransitionOn("event").To(StateA);
			smb.AddState(StateD);
			smb.TransitionOn("outer_event").To(StateD);

			target = new DelegateObject() { Execute = (s) => lastDelegateResult = s };
		}

		[Test]
		public void Test_Create_StateDelegate_OnBuilder()
		{
			smb.StateDelegate(target);

			CreateStateMachine();
			sm.Start();
		}

		[Test]
		public void Test_BasicStateDelegate()
		{
			smb.StateDelegate(target);
			CreateStateMachine();
			sm.Start();

			Assert.AreEqual("OnStateBEnter", lastDelegateResult);
		}

		[Test]
		public void Test_StateDelegate_WithTransition()
		{
			smb.StateDelegate(target);
			CreateStateMachine();
			sm.Start();

			Assert.AreEqual("OnStateBEnter", lastDelegateResult);
			sm.PostEvent("event");
			Assert.AreEqual("OnStateAExit", lastDelegateResult);
			sm.PostEvent("event");
			Assert.AreEqual("OnStateBEnter", lastDelegateResult);
		}

		[Test]
		public void Test_StateDelegate_LateDelegateAdd()
		{
			CreateStateMachine();
			sm.Start();

			Assert.IsNullOrEmpty(lastDelegateResult);
			sm.PostEvent("event");
			Assert.IsNullOrEmpty(lastDelegateResult);

			sm.AddStateDelegate(target);

			sm.PostEvent("event");
			Assert.AreEqual("OnStateBEnter", lastDelegateResult);
		}

		[Test]
		public void Test_StateDelegate_Removal()
		{
			smb.StateDelegate(target);
			CreateStateMachine();
			sm.Start();

			Assert.AreEqual("OnStateBEnter", lastDelegateResult);

			sm.RemoveStateDelegate(target);
			lastDelegateResult = null;
			sm.PostEvent("event");
			Assert.IsNullOrEmpty(lastDelegateResult);
		}

		[Test]
		public void Test_StateDelegate_ShorthandEnter()
		{
			smb.StateDelegate(target);
			CreateStateMachine();
			sm.Start();
			sm.PostEvent("outer_event");

			Assert.AreEqual("OnStateDEnter", lastDelegateResult);
		}
	}

	public class DelegateObject
	{
		public Action<string> Execute;

		public void OnStateAEnter() { Execute("OnStateAEnter"); }
		public void OnStateBEnter() { Execute("OnStateBEnter"); }
		public void OnStateAExit() { Execute("OnStateAExit"); }
		public void OnStateBExit() { Execute("OnStateBExit"); }
		public void OnStateD() { Execute("OnStateDEnter"); }
	}
}
