using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public class EventProcessor
	{
		private const int NotProcessing = 0;
		private const int Processing = 1;

		private ThreadSafeQueue<EventInstance> events;
		private int processingIndicator;

		public EventProcessor()
		{
			events = new ThreadSafeQueue<EventInstance>();
			processingIndicator = NotProcessing;
		}

		public void AddEvent(EventInstance eventToAdd)
		{
			events.Enqueue(eventToAdd);
		}

		public bool HasEvents { get { return events.HasItems; } }

		public bool CanProcess
		{
			get { return HasEvents && processingIndicator == NotProcessing; }
		}

		public State ProcessNextEvent(State currentState)
		{
			// Processing should not be re-entrant, on the same thread or otherwise
			if (NotProcessing == Interlocked.CompareExchange(ref processingIndicator, Processing, NotProcessing))
			{
				EventInstance eventToProcess = events.Dequeue();
				if (eventToProcess != null)
					currentState = currentState.ProcessEvent(currentState, eventToProcess);

				processingIndicator = NotProcessing;
			}
			
			return currentState;
		}
	}
}
