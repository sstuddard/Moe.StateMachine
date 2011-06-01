using Moe.StateMachine.Extensions.Logger;
using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class HistoryStateTests : BaseTest
	{
		[Test]
		public void Test_ShallowHistory_BasicSuccess()
		{
			smb.AddState(States.GreenParent)
				.InitialState()
					.AddState(States.Green)
					.InitialState()
					.TransitionOn(Events.Change).To(States.Yellow)
					.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent]
				.AddState(States.Yellow)
				.TransitionOn(Events.Change).To(States.Red)
				.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent]
				.AddState(States.Red)
				.TransitionOn(Events.Change).To(States.Green)
				.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent]
				.AddHistory();

			smb.AddState(States.Gold)
				.TransitionOn(Events.Change).ToHistory(States.GreenParent);

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Panic);		// To Gold
			sm.PostEvent(Events.Change);	// To history - Should go to yellow
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_ShallowHistory_DoesNotGoDeep()
		{
			smb.AddState(States.GreenParent)
				.InitialState()
					.AddState(States.Green)
					.InitialState()
						.AddState(States.GreenChild)
						.InitialState()
						.TransitionOn(Events.Change).To(States.Yellow)
						.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent][States.Green]
				.AddState(States.Yellow)
				.TransitionOn(Events.Change).To(States.Red)
				.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent][States.Green]
				.AddState(States.Red)
				.TransitionOn(Events.Change).To(States.GreenChild)
				.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent]
				.AddHistory();

			smb.AddState(States.Gold)
				.TransitionOn(Events.Change).ToHistory(States.GreenParent);

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Panic);		// To Gold
			Assert.IsTrue(sm.InState(States.Gold));
			sm.PostEvent(Events.Change);	// To history - Should go to greenchild
			Assert.IsTrue(sm.InState(States.GreenChild));
		}

		[Test]
		public void Test_ShallowHistory_NoHistoryDoesDefault()
		{
			smb.AddState(States.GreenParent)
				.InitialState()
				.AddState(States.Green)
					.InitialState()
					.TransitionOn(Events.Change).To(States.Yellow);
			smb[States.GreenParent]
				.AddState(States.Yellow)
					.TransitionOn(Events.Change).To(States.Red);
			smb[States.GreenParent]
				.AddState(States.Red)
					.TransitionOn(Events.Change).ToHistory(States.GreenParent);
			smb[States.GreenParent]
				.AddHistory();

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			sm.PostEvent(Events.Change);	// To history - Should go back to red
			Assert.IsTrue(sm.InState(States.Green));
		}

		[Test]
		public void Test_ShallowHistory_FromWithinToSuperstateHistory()
		{
			smb.AddState(States.GreenGrandParent)
				.InitialState()
				.AddState(States.GreenParent)
					.InitialState()
					.AddState(States.Green)
						.InitialState()
						.TransitionOn(Events.Change).To(States.Yellow);
			smb[States.GreenParent]
				.AddState(States.Yellow)
					.TransitionOn(Events.Change).To(States.Red);
			smb[States.GreenParent]
				.AddState(States.Red)
					.TransitionOn(Events.Change).To(States.Green);
			smb[States.GreenParent]
				.AddHistory()
				.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.Gold]
				.TransitionOn(Events.Change).ToHistory(States.GreenParent);

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Change);	// To Red
			sm.PostEvent(Events.Panic);		// To gold
			sm.PostEvent(Events.Change);	// To History of GreenParent => should return to red
			Assert.IsTrue(sm.InState(States.Red));
		}

		[Test]
		public void Test_DeepHistory_BasicSuccess()
		{
			smb.AddState(States.GreenParent)
				.InitialState()
					.AddState(States.Green)
					.InitialState()
					.TransitionOn(Events.Change).To(States.Yellow)
					.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent]
				.AddState(States.Yellow)
				.TransitionOn(Events.Change).To(States.Red)
				.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent]
				.AddState(States.Red)
				.TransitionOn(Events.Change).To(States.Green)
				.TransitionOn(Events.Panic).To(States.Gold);
			smb[States.GreenParent]
				.AddDeepHistory();

			smb.AddState(States.Gold)
				.TransitionOn(Events.Change).ToHistory(States.GreenParent);

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);	// To Yellow
			sm.PostEvent(Events.Panic);		// To Gold
			sm.PostEvent(Events.Change);	// To history - Should go to yellow
			Assert.IsTrue(sm.InState(States.Yellow));
		}

		[Test]
		public void Test_DeepHistory_GoesDeeeeeep()
		{
			smb.DefaultTransition(States.Gold);
			smb[States.GreenGrandParent].TransitionOn(Events.Change).ToHistory(States.Green);
			smb[States.Green].AddDeepHistory();
			smb[States.Green][States.Yellow][States.Red][States.RedChild][States.Gold]
				.TransitionOn(Events.Change, States.GreenGrandParent);

			CreateStateMachine();
			sm.Start();

			Assert.IsTrue(sm.InState(States.Gold));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.GreenGrandParent));
			sm.PostEvent(Events.Change);
			Assert.IsTrue(sm.InState(States.Gold));
			Assert.IsTrue(sm.InState(States.RedChild));
			Assert.IsTrue(sm.InState(States.Red));
			Assert.IsTrue(sm.InState(States.Yellow));
			Assert.IsTrue(sm.InState(States.Green));
		}
	}
}
