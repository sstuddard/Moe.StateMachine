using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Moe.StateMachine.Events;
using Moe.StateMachine.States;

namespace Moe.StateMachine
{
	public class AsyncStateMachine : StateMachine
	{
		private Thread stateThread;
		private AutoResetEvent eventsQueued;

		public AsyncStateMachine()
		{
			this.eventsQueued = new AutoResetEvent(false);
		}

		public AsyncStateMachine(StateMachine sm)
			: this()
		{
			// Cannot instantiate if start marchine already started
			if (sm.CurrentState != null)
				throw new ArgumentException();

			this.root = sm.RootNode;
			this.rootBuilder = sm.CreateStateBuilder(root);
		}

		public override void Start()
		{
			base.Start();

			stateThread = new Thread(RunMachine);
			stateThread.Start();
		}

		public override void PostEvent(object eventToPost)
		{
			eventHandler.AddEvent(new EventInstance(eventToPost));
			eventsQueued.Set();
		}

		public override void RegisterTimer(State s, DateTime timeout)
		{
			base.RegisterTimer(s, timeout);
			eventsQueued.Set();
		}

		private void RunMachine()
		{
			try
			{
				while (true)
				{
					// How much time until the next timeout?
					TimeSpan nextTimeout = timers.GetTimeToNextTimeout();
					int wait = Math.Min(500, Convert.ToInt32(nextTimeout.TotalMilliseconds));

					eventsQueued.WaitOne(wait);
					UpdateTimers();
					while (eventHandler.CanProcess)
						current = eventHandler.ProcessNextEvent(current);
				}
			}
			catch (ThreadAbortException)
			{
				return;
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			if (stateThread != null)
			{
				stateThread.Abort();
				stateThread.Join();
			}
		}
	}
}
