using Moe.StateMachine.States;

namespace Moe.StateMachine.Builders
{
	public interface IBaseStateBuilder
	{
		object Id { get; }
		State Build(State parent);
	}
}
