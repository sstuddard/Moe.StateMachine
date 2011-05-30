using System.Collections.Generic;
using Moe.StateMachine.Extensions;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Builders
{
	public interface IStateBuilderContext
	{
		State Resolve(object id);

		bool HasPlugIn<T>() where T : IPlugIn;
		T GetPlugIn<T>() where T : IPlugIn;
		void AddPlugIn(IPlugIn plugin);

		IEnumerable<IPlugIn> PlugIns { get; }
	}
}
