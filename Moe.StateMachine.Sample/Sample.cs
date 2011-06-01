using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Moe.StateMachine.Builders;
using Moe.StateMachine.Extensions.Asynchronous;
using Moe.StateMachine.Extensions.Timers;

namespace Moe.StateMachine.Sample
{
	public partial class Sample : Form
	{
		private enum States
		{
			Green,
			Yellow,
			Red
		}

		private new enum Events
		{
			Change
		}

		private StateMachineBuilder stateMachineBuilder;
		private StateMachine stateMachine;
		private readonly Color GreenOff = Color.FromArgb(192, 255, 192);
		private readonly Color GreenOn = Color.Lime;
		private readonly Color YellowOff = Color.FromArgb(255, 255, 192);
		private readonly Color YellowOn = Color.Yellow;
		private readonly Color RedOff = Color.FromArgb(255, 192, 192);
		private readonly Color RedOn = Color.Red;

		public Sample()
		{
			InitializeComponent();

			stateMachineBuilder = new StateMachineBuilder();
			stateMachineBuilder.AddState(States.Green)
				.TransitionOn(Events.Change, States.Yellow)
				.OnEnter(s => Invoker(() => pnlGreen.BackColor = GreenOn))
				.OnExit(s => Invoker(() => pnlGreen.BackColor = GreenOff))
				.Timeout(3000, States.Yellow)
				.InitialState();
			stateMachineBuilder.AddState(States.Yellow)
				.TransitionOn(Events.Change, States.Red)
				.Timeout(3000, States.Red)
				.OnEnter(s => Invoker(() => pnlYellow.BackColor = YellowOn))
				.OnExit(s => Invoker(() => pnlYellow.BackColor = YellowOff));
			stateMachineBuilder.AddState(States.Red)
				.TransitionOn(Events.Change, States.Green)
				.Timeout(3000, States.Green)
				.OnEnter(s => Invoker(() => pnlRed.BackColor = RedOn))
				.OnExit(s => Invoker(() => pnlRed.BackColor = RedOff));

			stateMachine = new StateMachine("StopLight", stateMachineBuilder);
			stateMachine.Asynchronous();
		}

		private void Invoker(Action action)
		{
			if (InvokeRequired)
				Invoke(action);
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			stateMachine.Start();
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		protected override void OnClosed(EventArgs e)
		{
			stateMachine.Dispose();
			base.OnClosed(e);
		}

		private void btnChange_Click(object sender, EventArgs e)
		{
			stateMachine.PostEvent(Events.Change);
		}
	}
}
