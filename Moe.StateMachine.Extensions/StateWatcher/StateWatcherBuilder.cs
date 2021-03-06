﻿using System;

namespace Moe.StateMachine.Extensions.StateWatcher
{
	public static class StateWatcherBuilder
	{
		public static CompositeStateWatcherAction AddStateWatcher(
			this StateMachine stateMachine, 
			Action<object> callback, 
			params object[] states)
		{
			CompositeStateWatcherAction newAction = 
				new CompositeStateWatcherAction(stateMachine, states, callback);

			return newAction;
		}
	}
}
