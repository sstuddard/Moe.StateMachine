using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jed.StateMachine
{
	internal class EventProcessor
	{
		private const int NotProcessing = 0;
		private const int Processing = 1;

		private ThreadSafeQueue<object> events;
		private int processingIndicator;

		public EventProcessor()
		{
			events = new ThreadSafeQueue<object>();
			processingIndicator = NotProcessing;
		}

		public void AddEvent(object eventToAdd)
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
				object eventToProcess = events.Dequeue();
				if (eventToProcess != null)
					currentState = currentState.ProcessEvent(eventToProcess);

				processingIndicator = NotProcessing;
			}
			
			return currentState;
		}
	}
}
