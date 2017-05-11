namespace KeepMeBusy
{
	partial class CommandTargetWindow
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
			this.Btn_Close = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Btn_Close
			// 
			this.Btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Btn_Close.Location = new System.Drawing.Point(24, 16);
			this.Btn_Close.Margin = new System.Windows.Forms.Padding(15, 7, 15, 7);
			this.Btn_Close.Name = "Btn_Close";
			this.Btn_Close.Size = new System.Drawing.Size(92, 27);
			this.Btn_Close.TabIndex = 0;
			this.Btn_Close.Text = "Close";
			this.Btn_Close.UseVisualStyleBackColor = true;
			// 
			// CommandTargetWindow
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.Btn_Close;
			this.ClientSize = new System.Drawing.Size(140, 59);
			this.ControlBox = false;
			this.Controls.Add(this.Btn_Close);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Location = new System.Drawing.Point(10, 10);
			this.Name = "CommandTargetWindow";
			this.Opacity = 0.5D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "86f115c7-4dd2-420b-9036-177ef7ed42a7";
			this.TopMost = true;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button Btn_Close;
	}
}