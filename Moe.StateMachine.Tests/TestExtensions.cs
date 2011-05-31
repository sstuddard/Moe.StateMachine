using NUnit.Framework;
using Moe.StateMachine.Extensions.StateWatcher;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestExtensions : BaseTest
	{
		[SetUp]
		public void Setup()
		{
			stateCalledBack = null;
		}

		[Test]
		public void Test_CreateBeforeStart_StateWatcher_OneState()
		{
			smb.AddState(States.Green).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.AddStateWatcher(StateCallback, States.Red);

			Assert.IsNull(stateCalledBack);
			sm.Start();
			Assert.IsNull(stateCalledBack);
			sm.PostEvent(Events.Change);
			Assert.IsNull(stateCalledBack);
			sm.PostEvent(Events.Change);
			Assert.AreEqual(stateCalledBack, States.Red);

			// Make sure it has disappeared
			stateCalledBack = null;
			sm.PostEvent(Events.Change);	// To Green
			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			Assert.IsNull(stateCalledBack);
		}

		[Test]
		public void Test_CreateBeforeStart_StateWatcher_MultipleState()
		{
			smb.AddState(States.Green).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.AddStateWatcher(StateCallback, States.Red, States.Yellow);

			Assert.IsNull(stateCalledBack);
			sm.Start();
			sm.PostEvent(Events.Change);
			Assert.AreEqual(States.Yellow, stateCalledBack);
			stateCalledBack = null;
			sm.PostEvent(Events.Change);
			Assert.IsNull(stateCalledBack);
		}

		[Test]
		public void Test_CreateAfterStart_StateWatcher_OneState()
		{
			smb.AddState(States.Green).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			// Do full cycle
			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			sm.PostEvent(Events.Change);	// To Green

			sm.AddStateWatcher(StateCallback, States.Red);

			Assert.IsNull(stateCalledBack);
			sm.PostEvent(Events.Change);
			Assert.IsNull(stateCalledBack);
			sm.PostEvent(Events.Change);
			Assert.AreEqual(stateCalledBack, States.Red);

			// Make sure it has disappeared
			stateCalledBack = null;
			sm.PostEvent(Events.Change);	// To Green
			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			Assert.IsNull(stateCalledBack);
		}

		[Test]
		public void Test_CreateAfterStart_StateWatcher_MultipleState()
		{
			smb.AddState(States.Green).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			// Do full cycle
			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			sm.PostEvent(Events.Change);	// To Green

			sm.AddStateWatcher(StateCallback, States.Red, States.Yellow);

			sm.PostEvent(Events.Change);
			Assert.AreEqual(States.Yellow, stateCalledBack);
			stateCalledBack = null;
			sm.PostEvent(Events.Change);	// To Red
			Assert.IsNull(stateCalledBack);
		}

		[Test]
		public void Test_CreateAfterStart_StateWatcher_OneSuperStateState()
		{
			smb.AddState(States.GreenParent).InitialState()
				.AddState(States.Green).TransitionOn(Events.Change, States.Yellow).InitialState();
			smb.AddState(States.Yellow).TransitionOn(Events.Change, States.Red);
			smb.AddState(States.Red).TransitionOn(Events.Change, States.Green);

			CreateStateMachine();
			sm.Start();

			stateCalledBack = null;
			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			sm.PostEvent(Events.Change);	// To Green
			Assert.IsNull(stateCalledBack);

			sm.AddStateWatcher(StateCallback, States.GreenParent);

			Assert.IsNull(stateCalledBack);
			sm.PostEvent(Events.Change);
			Assert.IsNull(stateCalledBack);
			sm.PostEvent(Events.Change);
			Assert.IsNull(stateCalledBack);
			sm.PostEvent(Events.Change);
			Assert.AreEqual(States.GreenParent, stateCalledBack);

			// Make sure it has disappeared
			stateCalledBack = null;
			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			sm.PostEvent(Events.Change);	// To Green
			Assert.IsNull(stateCalledBack);
		}

		private object stateCalledBack;
		private void StateCallback(object stateId)
		{
			stateCalledBack = stateId;
		}
	}
}
