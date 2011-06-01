using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class ActionsTest : BaseTest
	{
		[Test]
		public void Test_Actions_CurrentStateIsCorrectDuringAction()
		{
			smb[States.Green]
				.InitialState()
				.OnEnter(tr => AssertIn(States.Green))
				.OnExit(tr => AssertIn(States.Green))
				.TransitionOn(Events.Change).To(States.Yellow);
			smb[States.Yellow]
				.OnEnter(tr => AssertIn(States.Yellow))
				.OnExit(tr => AssertIn(States.Yellow))
				.TransitionOn(Events.Change).To(States.Green);

			CreateStateMachine();
			sm.Start();

			sm.PostEvent(Events.Change);
		}

		private void AssertIn(object state)
		{
			Assert.IsTrue(sm.InState(state));
		}
	}
}
