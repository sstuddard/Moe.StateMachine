namespace Moe.StateMachine.Actions
{
	public interface IActionPerformer
	{
		void Perform(TransitionReceipt transitionReceipt);
	}
}
