using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moe.StateMachine.Extensions.StateWatcher;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class TestExtensions
	{
		[SetUp]
		public void Setup()
		{
			stateCalledBack = null;
		}

		[Test]
		public void Test_CreateBeforeStart_StateWatcher_OneState()
		{
			var sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

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
			var sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

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
			var sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

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
			var sm = new StateMachine();
			sm.AddState(States.Green).TransitionTo(Events.Change, States.Yellow).InitialState();
			sm.AddState(States.Yellow).TransitionTo(Events.Change, States.Red);
			sm.AddState(States.Red).TransitionTo(Events.Change, States.Green);

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

		private object stateCalledBack;
		private void StateCallback(object stateId)
		{
			stateCalledBack = stateId;
		}
	}
}
