using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moe.StateMachine.Extensions.Logger;

namespace Moe.StateMachine.Extensions.Timers
{
	public class TimerPlugin : IPlugIn, IDisposable
	{
		private StateMachine stateMachine;
		private readonly AutoResetEvent alarmReady;
		private Thread stateThread;
		private LoggerPlugIn logger;

		private List<ITimer> timers;

		public TimerPlugin()
		{
			alarmReady = new AutoResetEvent(false);
			timers = new List<ITimer>();
		}

		private void Start()
		{
			logger = stateMachine.GetPlugIn<LoggerPlugIn>();

			stateThread = new Thread(RunAlarm);
			stateThread.Start();
		}

		private void Stop()
		{
			if (stateThread != null)
			{
				stateThread.Abort();
				stateThread.Join();
			}
		}

		public void Initialize(StateMachine sm)
		{
			stateMachine = sm;
			stateMachine.Starting += delegate { Start(); };
			stateMachine.Stopping += delegate { Stop(); };

			if (stateMachine.IsRunning)
				Start();
		}

		public void PostEvent(object eventToPost)
		{
			stateMachine.PostEvent(eventToPost);
		}

		public void AddTimer(ITimer timer)
		{
			lock (timers)
			{
				timers.Add(timer);
			}

			timer.ActiveChanged += OnActiveTimerChanged;
		}

		public void RemoveTimer(ITimer timer)
		{
			timer.ActiveChanged -= OnActiveTimerChanged;

			lock (timers)
			{
				timers.Remove(timer);
			}
		}

		private void RunAlarm()
		{
			try
			{
				while (true)
				{
					ITimer next;
					while ((next = GetNextAlarm()) != null)
					{
						Log("Next alarm in {0}ms", (next.NextTime - DateTime.Now).TotalMilliseconds);
						if (next.NextTime < DateTime.Now)
							next.Execute();
						else
							break;
					}

					TimeSpan wait = TimeSpan.FromMilliseconds(500);
					if (next != null) 
						wait = next.NextTime - DateTime.Now;
					alarmReady.WaitOne(wait);
				}
			}
			catch (ThreadAbortException)
			{
				return;
			}
		}

		private void OnActiveTimerChanged(object sender, EventArgs args)
		{
			alarmReady.Set();
		}

		private ITimer GetNextAlarm()
		{
			List<ITimer> activeTimers;
			lock (timers)
			{
				activeTimers = new List<ITimer>(timers.Where(t => t.Active));
			}

			if (activeTimers.Count > 0)
			{
				activeTimers.Sort((t1, t2) => t1.NextTime.CompareTo(t2.NextTime));
				ITimer next = activeTimers[0];
				return next;
			}

			return null;
		}

		public void Dispose()
		{
			Stop();
		}

		private void Log(string message, params object[] messageParams)
		{
			if (logger != null)
				logger.Log(message, messageParams);
		}
	}
}
