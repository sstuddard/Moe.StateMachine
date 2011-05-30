using System;
using System.Threading;
using Moe.StateMachine.Events;

namespace Moe.StateMachine.Extensions.Asynchronous
{
	public class AsynchronousPlugIn : EventProcessor, IPlugIn, IDisposable
	{
		private readonly AutoResetEvent eventsQueued;
		private StateMachine stateMachine;
		private Thread stateThread;

		public AsynchronousPlugIn()
		{
			eventsQueued = new AutoResetEvent(false);
		}

		private void Start()
		{
			stateThread = new Thread(RunMachine);
			stateThread.Start();
		}

		public void Initialize(StateMachine sm)
		{
			stateMachine = sm;
			stateMachine.Starting += delegate { Start(); };
			stateMachine.Stopping += delegate { Stop(); };
		}

		private void RunMachine()
		{
			try
			{
				while (true)
				{
					// How much time until the next timeout?
					eventsQueued.WaitOne(500);
					while (CanProcess)
						ProcessNextEvent(stateMachine.CurrentState);
				}
			}
			catch (ThreadAbortException)
			{
				return;
			}
		}

		private void Stop()
		{
			if (stateThread != null)
			{
				stateThread.Abort();
				stateThread.Join();
			}
		}

		public void Dispose()
		{
			Stop();
		}
	}
}
