using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Jed.StateMachine.Sample
{
	public partial class Sample : Form
	{
		private enum States
		{
			Green,
			Yellow,
			Red
		}

		private enum Events
		{
			Change
		}

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

			stateMachine = new AsyncStateMachine();
			stateMachine.AddState(States.Green)
				.TransitionTo(Events.Change, States.Yellow)
				.OnEnter(s => pnlGreen.BackColor = GreenOn)
				.OnExit(s => pnlGreen.BackColor = GreenOff)
				.Timeout(3000, States.Yellow)
				.InitialState();
			stateMachine.AddState(States.Yellow)
				.TransitionTo(Events.Change, States.Red)
				.Timeout(3000, States.Red)
				.OnEnter(s => pnlYellow.BackColor = YellowOn)
				.OnExit(s => pnlYellow.BackColor = YellowOff);
			stateMachine.AddState(States.Red)
				.TransitionTo(Events.Change, States.Green)
				.Timeout(3000, States.Green)
				.OnEnter(s => pnlRed.BackColor = RedOn)
				.OnExit(s => pnlRed.BackColor = RedOff);
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			stateMachine.Start();
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			stateMachine.Dispose();
			Close();
		}

		private void btnChange_Click(object sender, EventArgs e)
		{
			stateMachine.PostEvent(Events.Change);
		}
	}
}
