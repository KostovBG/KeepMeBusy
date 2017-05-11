using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;

namespace KeepMeBusy
{
	public class BusySimulatorUI : IDisposable
	{
		#region IDisposable Implementation
		private bool disposed = false;

		//Implement IDisposable.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					SystemEvents.SessionSwitch -= this.SessionLockHandler;

					if (this.components != null) { this.components.Dispose(); }
				}

				disposed = true;
			}
		}
		#endregion

		#region PROPERTIES

		private const int TrayIconWarningTimeMinutes = 5;
		public const int TimerInterval = 60000; // 60 seconds
		private const int BalloonTipDuration = 1000;

		private BusySimulatorConfig AppConfig;

		private NotifyIcon SystemTrayIcon;
		private System.Timers.Timer MinuteTimer;
		private IContainer components = null;

		private SessionSwitchEventHandler SessionLockHandler;

		private ContextMenuStrip Context_Menu;
		private ToolStripMenuItem MenuItem_IsEnabled;
		private ToolStripMenuItem MenuItem_IsRefreshEnabled;
		private ToolStripSeparator MenuItem_Separator1;
		private ToolStripSeparator MenuItem_Separator2;
		private ToolStripMenuItem MenuItem_AutoRunAtLogon;
		private ToolStripMenuItem MenuItem_Close;

		private bool SessionCurrentlyLocked { get; set; }

		#endregion

		public BusySimulatorUI()
		{
			this.AppConfig = new BusySimulatorConfig();

			this.SessionLockHandler = new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
			SystemEvents.SessionSwitch += this.SessionLockHandler;

			InitUIControls();

			this.AppConfig.PropertyChanged += AppConfig_PropertyChanged;
			this.AppConfig.AutoSavePropertyValuesInRegistry = true;

			this.AppConfig.RefreshUI();
		}

		private void AppConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "TrayIcon":
					this.SystemTrayIcon.Icon = this.AppConfig.TrayIcon;
					break;

				case "TrayIconToolTip":
					this.SystemTrayIcon.Text = this.AppConfig.TrayIconToolTip;
					break;

				case "IsEnabled":
					this.MenuItem_IsEnabled.Checked = this.AppConfig.IsEnabled;
					this.MenuItem_IsRefreshEnabled.Enabled = this.AppConfig.IsEnabled;
					this.MenuItem_IsRefreshEnabled.Checked = this.AppConfig.IsRefreshEnabled;
					break;

				case "IsRefreshEnabled":
					this.MenuItem_IsRefreshEnabled.Checked = this.AppConfig.IsRefreshEnabled;
					break;

				case "AutoRunAtLogon":
					this.MenuItem_AutoRunAtLogon.Checked = this.AppConfig.AutoRunAtLogon;
					break;

				default:
					break;
			}
		}

		private void InitUIControls()
		{
			#region create instances
			this.components = new Container();
			this.Context_Menu = new ContextMenuStrip(this.components);
			this.SystemTrayIcon = new NotifyIcon(components);
			this.MenuItem_IsEnabled = new ToolStripMenuItem();
			this.MenuItem_IsRefreshEnabled = new ToolStripMenuItem();
			this.MenuItem_Separator1 = new ToolStripSeparator();
			this.MenuItem_Separator2 = new ToolStripSeparator();
			this.MenuItem_AutoRunAtLogon = new ToolStripMenuItem();
			this.MenuItem_Close = new ToolStripMenuItem();

			#endregion

			this.Context_Menu.SuspendLayout();

			#region Base Menu
			// Context_Menu
			this.Context_Menu.Items.AddRange(new ToolStripItem[] {
			this.MenuItem_IsEnabled,
			this.MenuItem_IsRefreshEnabled,
			this.MenuItem_Separator1,
			this.MenuItem_AutoRunAtLogon,
			this.MenuItem_Separator2,
			this.MenuItem_Close});
			this.Context_Menu.Name = "Context Menu";
			this.Context_Menu.Size = new System.Drawing.Size(228, 92);

			// MenuItem_SetBusyState
			this.MenuItem_IsEnabled.Name = "IsEnabled";
			this.MenuItem_IsEnabled.Size = new System.Drawing.Size(177, 22);
			this.MenuItem_IsEnabled.Text = "Simulate Busy State";
			this.MenuItem_IsEnabled.ToolTipText = "Prevents the system from entering sleep or " + Environment.NewLine +
													 "turning off the display while the application is running.";
			this.MenuItem_IsEnabled.Click += (s, e) => { Switch_IsEnabled(); };

			// MenuItem_EnableTimer
			this.MenuItem_IsRefreshEnabled.Name = "IsRefreshEnabled";
			this.MenuItem_IsRefreshEnabled.Size = new System.Drawing.Size(227, 22);
			this.MenuItem_IsRefreshEnabled.Text = "Refresh state every minute";
			this.MenuItem_IsRefreshEnabled.ToolTipText = "Useful for some Instant Messaging programs." + Environment.NewLine + "Simulates a simple user interaction every minute.";
			this.MenuItem_IsRefreshEnabled.Enabled = false;
			this.MenuItem_IsRefreshEnabled.Click += (s, e) => {
				this.AppConfig.IsRefreshEnabled = !(this.AppConfig.IsRefreshEnabled);
				ShowBaloonTip();
			};

			// MenuItem_AutoStartAtLogon
			this.MenuItem_AutoRunAtLogon.Name = "AutoRunAtLogon";
			this.MenuItem_AutoRunAtLogon.Size = new System.Drawing.Size(227, 22);
			this.MenuItem_AutoRunAtLogon.Text = "Auto Start at Logon";
			this.MenuItem_AutoRunAtLogon.Enabled = true;
			this.MenuItem_AutoRunAtLogon.Click += (s, e) => {
				this.AppConfig.AutoRunAtLogon = !(this.AppConfig.AutoRunAtLogon);
			};

			// MenuItem_Close
			this.MenuItem_Close.Name = "Close";
			this.MenuItem_Close.Size = new System.Drawing.Size(177, 22);
			this.MenuItem_Close.Text = "Close";
			this.MenuItem_Close.Click += (s, e) => { AppClose(); };

			#endregion

			// SystemTrayIcon
			this.SystemTrayIcon.Visible = true;
			this.SystemTrayIcon.Icon = this.AppConfig.TrayIcon;
			this.SystemTrayIcon.Text = this.AppConfig.TrayIconToolTip;
			this.SystemTrayIcon.DoubleClick += SystemTrayIcon_DoubleClick;
			this.SystemTrayIcon.ContextMenuStrip = this.Context_Menu;

			this.MinuteTimer = new System.Timers.Timer();
			this.MinuteTimer.Elapsed += MinuteTimer_Elapsed;
			this.MinuteTimer.AutoReset = true;
			this.MinuteTimer.Interval = TimerInterval;
			this.MinuteTimer.Start();

			this.Context_Menu.Renderer = new UIToolStripMenuRenderer();
			this.Context_Menu.ResumeLayout(false);
		}
		private void SystemTrayIcon_DoubleClick(object sender, EventArgs e)
		{
			// TBD
			ShowBaloonTip();
		}
		private void MinuteTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if ((this.AppConfig.IsRefreshEnabled) && (!this.SessionCurrentlyLocked))
			{
				CommandTargetWindow cmdTarget = new CommandTargetWindow();

				ThreadStateController.SetBusy();
				ThreadStateController.SetForegroundWindow(cmdTarget.Handle);
				SendKeys.SendWait("%1");

				cmdTarget.Close();

#if DEBUG
				this.SystemTrayIcon.BalloonTipText = "Busy Refresh";
				this.SystemTrayIcon.ShowBalloonTip(1000);
#endif
			}
		}

		private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason == SessionSwitchReason.SessionLock)
			{
				this.SessionCurrentlyLocked = true;
			}
			else if (e.Reason == SessionSwitchReason.SessionUnlock)
			{
				this.SessionCurrentlyLocked = false;
				ShowBaloonTip();
			}
		}
		private void Switch_IsEnabled()
		{
			if(this.AppConfig.IsEnabled)
			{
				if(ThreadStateController.SetDefault())
				{
					this.AppConfig.IsEnabled = false;
				}
				else
				{
					// TBD: Show Error
				}
			}
			else
			{
				if (ThreadStateController.SetBusy())
				{
					this.AppConfig.IsEnabled = true;
				}
				else
				{
					// TBD: Show Error
				}
			}

			this.AppConfig.RefreshUI();
			ShowBaloonTip();
		}
		
		private void ShowBaloonTip()
		{
			this.SystemTrayIcon.Visible = false;
			this.SystemTrayIcon.Visible = true;
			this.SystemTrayIcon.BalloonTipText = this.AppConfig.TrayIconToolTip;
			this.SystemTrayIcon.BalloonTipIcon = ToolTipIcon.Info;
			this.SystemTrayIcon.ShowBalloonTip(BalloonTipDuration);
		}

		#region App Close
		private void AppClose()
		{
			ThreadStateController.SetDefault();
			CloseProgram();
		}
		public void CloseProgram()
		{
			if (Application.MessageLoop)
			{
				Application.Exit();
			}
			else
			{
				// Console app
				Environment.Exit(0);
			}
		}
		#endregion
	}
}
