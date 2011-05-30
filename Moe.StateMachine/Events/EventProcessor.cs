using System;
using System.Linq;
using System.Threading;
using Moe.StateMachine.States;

namespace Moe.StateMachine.Events
{
	public abstract class EventProcessor
	{
		public event EventHandler<StateEventPostedArgs> EventProcessed;

		private const int NotProcessing = 0;
		private const int Processing = 1;

		protected ThreadSafeQueue<EventInstance> events;
		private int processingIndicator;

		protected EventProcessor()
		{
			events = new ThreadSafeQueue<EventInstance>();
			processingIndicator = NotProcessing;
			EventProcessed += delegate { };
		}

		public virtual void AddEvent(EventInstance eventToAdd)
		{
			events.Enqueue(eventToAdd);
		}

		public bool HasEvents { get { return events.HasItems; } }

		public bool CanProcess
		{
			get { return HasEvents && processingIndicator == NotProcessing; }
		}

		public void ProcessNextEvent(State currentState)
		{
			// Processing should not be re-entrant, on the same thread or otherwise
			if (NotProcessing == Interlocked.CompareExchange(ref processingIndicator, Processing, NotProcessing))
			{
				EventInstance eventToProcess = events.Dequeue();
				if (eventToProcess != null)
					currentState = currentState.ProcessEvent(currentState, eventToProcess);

				processingIndicator = NotProcessing;

				if (currentState == null || currentState.Substates.Count() > 0)
					throw new InvalidOperationException("Current state [" + currentState + "] is a superstate or null.");

				EventProcessed(this, new StateEventPostedArgs(currentState, eventToProcess));
			}
		}
	}
}
