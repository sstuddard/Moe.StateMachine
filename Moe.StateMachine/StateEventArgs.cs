using System;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public class StateEventArgs : EventArgs
	{
		public State State { get; private set; }

		public StateEventArgs(State state)
		{
			State = state;
		}
	}

	public class StateEventPostedArgs : StateEventArgs
	{
		public EventInstance Event { get; private set; }

		public StateEventPostedArgs(State state, EventInstance eventPosted)
			: base(state)
		{
			Event = eventPosted;
		}
	}

	public class EventPostedArgs : EventArgs
	{
		public EventInstance Event { get; private set; }

		public EventPostedArgs(EventInstance eventPosted)
		{
			Event = eventPosted;
		}
	}
}
