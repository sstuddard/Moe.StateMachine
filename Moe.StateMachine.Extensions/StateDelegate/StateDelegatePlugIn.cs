using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Extensions.StateDelegate
{
	/// <summary>
	/// Plugin allows for adding delegate objects that follow a convention for state entry/exit notification.
	/// A delegate, to receive notification of a state entered/exited, must provide a public method that is named
	/// like this:  public void On<StateString>Enter(object event) or no parameters.  Exit notifications work the
	/// same.  In the example above, the "StateString" is the state object ToString() result.
	/// </summary>
	public class StateDelegatePlugIn : IPlugIn
	{
		private readonly Dictionary<object, StateDelegateDescriptor> _delegates;
		private bool _initialized;
		private StateMachine _sm;

		public StateDelegatePlugIn()
		{
			_delegates = new Dictionary<object, StateDelegateDescriptor>();
			_initialized = false;
		}

		public void Initialize(StateMachine sm)
		{
			_sm = sm;
			if (_sm.IsRunning)
				InitializeStates();
			else
			{
				_sm.Starting += (s, e) => InitializeStates();
			}
		}

		public void AddDelegate(object target)
		{
			_delegates[target] = new StateDelegateDescriptor(target);
		}

		public void RemoveDelegate(object target)
		{
			if (_delegates.ContainsKey(target))
				_delegates.Remove(target);
		}

		private void InitializeStates()
		{
			if (!_initialized)
			{
				_initialized = true;
				AttachState(_sm.RootNode);
			}
		}

		private void AttachState(State state)
		{
			state.Entered += OnStateEntered;
			state.Exited += OnStateExited;
			foreach (State child in state.Substates)
				AttachState(child);
		}

		private void OnStateEntered(object sender, StateTransitionEventArgs e)
		{
			foreach (var descriptor in _delegates.Values)
			{
				descriptor.DelegateEnter(((State)sender).Id, e.TransitionEvent.EventInstance.Event);
			}
		}

		private void OnStateExited(object sender, StateTransitionEventArgs e)
		{
			foreach (var descriptor in _delegates.Values)
			{
				descriptor.DelegateExit(((State)sender).Id, e.TransitionEvent.EventInstance.Event);
			}
		}
	}
}
