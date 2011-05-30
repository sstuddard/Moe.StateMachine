using System;
using Moe.StateMachine.Extensions.Actions;

namespace Moe.StateMachine
{
	public static class ActionBuilder
	{
		public static IStateBuilder OnEnter(this IStateBuilder builder, Action<TransitionReceipt> action)
		{
			builder.AddSecondPassAction(s =>
			                            	{
			                            		new EnterAction(s, action);
			                            	});
			return builder;
		}

		public static IStateBuilder OnExit(this IStateBuilder builder, Action<TransitionReceipt> action)
		{
			builder.AddSecondPassAction(s =>
											{
												new ExitAction(s, action);
											});
			return builder;
		}
	}
}
