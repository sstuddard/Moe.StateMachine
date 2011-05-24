using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moe.StateMachine.Tests
{
	public enum States
	{
		Green,
		GreenParent,
		GreenChild,
		GreenChild2,
		GreenGrandChild,
		Red,
		RedParent,
		RedChild,
		Yellow,
		YellowChild,
		Gold
	}

	public enum Events
	{
		Change,
		Panic,
		Pulse
	}
}
