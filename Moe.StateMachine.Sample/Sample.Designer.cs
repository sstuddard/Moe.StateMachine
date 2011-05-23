namespace Moe.StateMachine.Sample
{
	partial class Sample
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnStart = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.pnlGreen = new System.Windows.Forms.Panel();
			this.pnlYellow = new System.Windows.Forms.Panel();
			this.pnlRed = new System.Windows.Forms.Panel();
			this.btnChange = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(12, 198);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnExit
			// 
			this.btnExit.Location = new System.Drawing.Point(13, 306);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(75, 23);
			this.btnExit.TabIndex = 1;
			this.btnExit.Text = "Exit";
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// pnlGreen
			// 
			this.pnlGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.pnlGreen.Location = new System.Drawing.Point(13, 12);
			this.pnlGreen.Name = "pnlGreen";
			this.pnlGreen.Size = new System.Drawing.Size(57, 56);
			this.pnlGreen.TabIndex = 2;
			// 
			// pnlYellow
			// 
			this.pnlYellow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.pnlYellow.Location = new System.Drawing.Point(13, 74);
			this.pnlYellow.Name = "pnlYellow";
			this.pnlYellow.Size = new System.Drawing.Size(57, 56);
			this.pnlYellow.TabIndex = 3;
			// 
			// pnlRed
			// 
			this.pnlRed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			this.pnlRed.Location = new System.Drawing.Point(13, 136);
			this.pnlRed.Name = "pnlRed";
			this.pnlRed.Size = new System.Drawing.Size(57, 56);
			this.pnlRed.TabIndex = 4;
			// 
			// btnChange
			// 
			this.btnChange.Location = new System.Drawing.Point(12, 227);
			this.btnChange.Name = "btnChange";
			this.btnChange.Size = new System.Drawing.Size(75, 23);
			this.btnChange.TabIndex = 5;
			this.btnChange.Text = "Change";
			this.btnChange.UseVisualStyleBackColor = true;
			this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
			// 
			// Sample
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(116, 341);
			this.Controls.Add(this.btnChange);
			this.Controls.Add(this.pnlRed);
			this.Controls.Add(this.pnlYellow);
			this.Controls.Add(this.pnlGreen);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnStart);
			this.Name = "Sample";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Panel pnlGreen;
		private System.Windows.Forms.Panel pnlYellow;
		private System.Windows.Forms.Panel pnlRed;
		private System.Windows.Forms.Button btnChange;
	}
}

