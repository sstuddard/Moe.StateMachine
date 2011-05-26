namespace Moe.StateMachine.Actions
{
	public enum ActionType
	{
		Exit,
		Enter
	}

	public interface IActionPerformer
	{
		ActionType Type { get; }
		void Perform(TransitionReceipt transitionReceipt);
	}
}
