using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Moe.StateMachine.Extensions.StateDelegate
{
	internal class StateDelegateDescriptor
	{
		private readonly object _delegate;
		private Dictionary<string, MethodInfo> _listeners;

		public StateDelegateDescriptor(object target)
		{
			_delegate = target;
		}

		private void EnsureListeners()
		{
			if (_listeners == null)
			{
				_listeners = new Dictionary<string, MethodInfo>();

				var delegateType = _delegate.GetType();
				foreach (var method in delegateType.GetMethods())
				{
					if (method.Name.StartsWith("On"))
					{
						_listeners[method.Name] = method;
					}
				}
			}
		}

		public void DelegateEnter(object state, object sourceEvent)
		{
			Delegate(String.Format("On{0}Enter", state.ToString()), sourceEvent);
			Delegate(String.Format("On{0}", state.ToString()), sourceEvent);
		}

		public void DelegateExit(object state, object sourceEvent)
		{
			Delegate(String.Format("On{0}Exit", state.ToString()), sourceEvent);
			
		}

		private void Delegate(string methodName, object sourceEvent)
		{
			try
			{
				EnsureListeners();
				if (_listeners.ContainsKey(methodName))
				{
					var method = _listeners[methodName];
					if (method.GetParameters().Count() == 1)
					{
						method.Invoke(_delegate, new[] { sourceEvent });
					}
					else if (method.GetParameters().Count() == 0)
					{
						method.Invoke(_delegate, null);
					}
				}
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("State delegation failed: " + e.Message);
			}
		}
	}
}
