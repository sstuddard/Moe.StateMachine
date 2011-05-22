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
					currentState = ProcessEvent(eventToProcess, currentState);

				processingIndicator = NotProcessing;
			}
			
			return currentState;
		}

		private State ProcessEvent(EventInstance eventToProcess, State currentState)
		{
			// Perform state transition
			TransitionInstance transition = eventToProcess.ProcessEvent(currentState);
			if (transition != null)
			{
				currentState = HandleDefaultTransitions(transition.Transition());
			}

			return currentState;
		}

		private State HandleDefaultTransitions(State currentState)
		{
			State previous;
			do
			{
				previous = currentState;
				currentState = ProcessEvent(new DefaultEventEntryInstance(), currentState);
			} while (currentState != previous);

			return currentState;
		}
	}
}
