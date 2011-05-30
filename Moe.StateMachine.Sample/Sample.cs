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
				.TransitionTo(Events.Change, States.Yellow)
				.OnEnter(s => pnlGreen.BackColor = GreenOn)
				.OnExit(s => pnlGreen.BackColor = GreenOff)
				.Timeout(3000, States.Yellow)
				.InitialState();
			stateMachineBuilder.AddState(States.Yellow)
				.TransitionTo(Events.Change, States.Red)
				.Timeout(3000, States.Red)
				.OnEnter(s => pnlYellow.BackColor = YellowOn)
				.OnExit(s => pnlYellow.BackColor = YellowOff);
			stateMachineBuilder.AddState(States.Red)
				.TransitionTo(Events.Change, States.Green)
				.Timeout(3000, States.Green)
				.OnEnter(s => pnlRed.BackColor = RedOn)
				.OnExit(s => pnlRed.BackColor = RedOff);

			stateMachine = new StateMachine(stateMachineBuilder);
			stateMachine.Asynchronous();
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
