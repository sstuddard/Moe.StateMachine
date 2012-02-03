using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Extensions.StateDelegate
{
	public static class StateDelegateBuilder
	{
		public static void AddStateDelegate(this StateMachine sm, object target)
		{
			EnableStateDelegates(sm).AddDelegate(target);
		}

		public static void RemoveStateDelegate(this StateMachine sm, object target)
		{
			EnableStateDelegates(sm).RemoveDelegate(target);
		}

		public static IStateBuilder StateDelegate(this IStateBuilder smb, object target)
		{
			if (!smb.Context.HasPlugIn<StateDelegatePlugIn>())
				smb.Context.AddPlugIn(new StateDelegatePlugIn());

			smb.Context.GetPlugIn<StateDelegatePlugIn>().AddDelegate(target);

			return smb;
		}

		public static StateDelegatePlugIn EnableStateDelegates(this StateMachine sm)
		{
			if (sm.GetPlugIn<StateDelegatePlugIn>() == null)
				sm.AddPlugIn(new StateDelegatePlugIn());

			return sm.GetPlugIn<StateDelegatePlugIn>();
		}
	}
}
