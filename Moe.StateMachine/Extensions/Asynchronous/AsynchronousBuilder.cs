namespace Moe.StateMachine.Extensions.Asynchronous
{
	public static class AsynchronousBuilder
	{
		public static StateMachine Asynchronous(this StateMachine sm)
		{
			if (sm.GetPlugIn<AsynchronousPlugIn>() == null)
				sm.AddPlugIn(new AsynchronousPlugIn());
			
			return sm;
		}
	}
}
