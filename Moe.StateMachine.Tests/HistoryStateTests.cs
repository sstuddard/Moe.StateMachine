using NUnit.Framework;

namespace Moe.StateMachine.Tests
{
	[TestFixture]
	public class HistoryStateTests
	{
		private StateMachineBuilder smb;

		[SetUp]
		public void Setup()
		{
			smb = new StateMachineBuilder();
		}

		[Test]
		public void Test_DeepHistory_BasicSuccess()
		{
			smb.AddState(States.GreenParent)
				.InitialState()
					.AddState(States.Green)
					.InitialState()
					.TransitionOn(Events.Change, States.Yellow)
					.TransitionOn(Events.Panic, States.Gold);
			smb[States.GreenParent]
				.AddState(States.Yellow)
				.TransitionOn(Events.Change, States.Red)
				.TransitionOn(Events.Panic, States.Gold);
			smb[States.GreenParent]
				.AddState(States.Red)
				.TransitionOn(Events.Change, States.Red)
				.TransitionOn(Events.Panic, States.Gold);
			//smb[States.GreenParent]
			//    .AddHistory();

			smb.AddState(States.Gold)
				.TransitionOn(Events.Change, States.GreenParentHistory);
		}
	}
}
