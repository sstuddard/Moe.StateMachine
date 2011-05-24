namespace Moe.StateMachine.Actions
{
	public interface IActionPerformer
	{
		void Perform(object stateId, object context);
	}
}
