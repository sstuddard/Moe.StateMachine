using System;

namespace Moe.StateMachine.Extensions.Timers
{
	public static class TimerBuilder
	{
		public static readonly object TimeoutEvent = "Timeout";

		public static IStateBuilder Timeout(this IStateBuilder builder, int timeoutInMilliseconds, object targetState)
		{
			builder.TransitionOn(TimeoutEvent, targetState);
			AddTimeout(builder, timeoutInMilliseconds);
			return builder;
		}

		public static IStateBuilder Timeout(this IStateBuilder builder, 
			int timeoutInMilliseconds, object targetState, Func<bool> guard)
		{
			builder.TransitionOn(TimeoutEvent).To(targetState).When(guard);
			AddTimeout(builder, timeoutInMilliseconds);
			return builder;
		}

		private static void AddTimeout(IStateBuilder builder, int timeoutInMilliseconds)
		{
			if (!builder.Context.HasPlugIn<TimerPlugin>())
				builder.Context.AddPlugIn(new TimerPlugin());
			if (!builder.Context.HasPlugIn<TimeoutPlugin>())
				builder.Context.AddPlugIn(new TimeoutPlugin());

			builder.AddSecondPassAction(s =>
			                            	{
			                            		var timerPlugin = builder.Context.GetPlugIn<TimerPlugin>();
			                            		var timeoutPlugin = builder.Context.GetPlugIn<TimeoutPlugin>();
												var timeout = new TimeoutEvent(timeoutInMilliseconds, s, timeoutPlugin.PostTimeout);

												timerPlugin.AddTimer(timeout);
											});
		}
	}
}