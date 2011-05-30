using System;
using System.Collections.Generic;
using Moe.StateMachine.Extensions;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Builders
{
	public class StateBuilderContext : IStateBuilderContext
	{
		private State root;
		private List<IPlugIn> plugins;

		public StateBuilderContext()
		{
			plugins = new List<IPlugIn>();
		}

		public IEnumerable<IPlugIn> PlugIns { get { return plugins; } }

		public void SetRootState(State root)
		{
			this.root = root;
		}

		public State Resolve(object id)
		{
			if (root == null)
				throw new InvalidOperationException("Cannot resolve states, building has not been started.");
			return root.GetState(id);
		}

		public T GetPlugIn<T>() where T : IPlugIn
		{
			return (T)plugins.Find(pi => pi is T);
		}

		public bool HasPlugIn<T>() where T : IPlugIn
		{
			return plugins.Find(pi => pi is T) != null;
		}

		public void AddPlugIn(IPlugIn plugin)
		{
			plugins.Add(plugin);
		}
	}
}
