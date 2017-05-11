using System;
using System.ComponentModel;
using Microsoft.Win32;

namespace KeepMeBusy
{
	public class BusySimulatorConfig : INotifyPropertyChanged
	{
		#region Notify Property Changed

		public event PropertyChangedEventHandler PropertyChanged;

		public void ClearEventSubscriptions()
		{
			PropertyChanged = null;
		}

		private void NotifyPropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChangedEventArgs args = new PropertyChangedEventArgs(name);
				this.PropertyChanged(this, args);
			}
		}

		#endregion

		public const int BusySimulatorTimerInterval = 1000;  // milliseconds
		public const string ConfigRegistryKey = @"Software\KeepMeBusy";
		private const string CurrentUserAutoRunRegistry = @"Software\Microsoft\Windows\CurrentVersion\Run";

		public BusySimulatorConfig()
		{
			this.AssemblyLocation = "\"" + (System.Reflection.Assembly.GetExecutingAssembly()).Location + "\"";

			GetAutoLoadAtLogonRegistry();
			LoadLastStateFromRegistry();
		}

		private string AssemblyLocation { get; set; }

		#region User Preferences
		private bool _AutoRunAtLogon;
		private bool _IsEnabled;
		private bool _IsRefreshEnabled;

		/// <summary>
		/// Shows if the program is configured to start automatically at logon.
		/// </summary>
		public bool AutoRunAtLogon
		{
			get { return this._AutoRunAtLogon; }
			set
			{
				if(this._AutoRunAtLogon != value)
				{
					this._AutoRunAtLogon = value;
					// Store Preference in Registry
					SetAutoLoadAtLogonRegistry(value);
					// Refresh UI
					NotifyPropertyChanged("AutoRunAtLogon");
				}
			}
		}
		/// <summary>
		/// Shows if the Busy Simulator is Active
		/// </summary>
		public bool IsEnabled
		{
			get { return this._IsEnabled; }
			set
			{
				if (this._IsEnabled != value)
				{
					this._IsEnabled = value;
					// Store Preference in Registry
					AutoSaveDWordSettingInRegistry("IsEnabled", (value ? 1 : 0));
					// Refresh UI
					NotifyPropertyChanged("IsEnabled");
					NotifyPropertyChanged("TrayIcon");
					NotifyPropertyChanged("TrayIconToolTip");
				}
			}
		}
		/// <summary>
		/// Shows if the Busy Simulator's automatic refresh is enabled.
		/// </summary>
		public bool IsRefreshEnabled
		{
			get { return this._IsRefreshEnabled; }
			set
			{
				if (this._IsRefreshEnabled != value)
				{
					this._IsRefreshEnabled = value;
					// Store Preference in Registry
					AutoSaveDWordSettingInRegistry("IsRefreshEnabled", (value ? 1 : 0));
					// Refresh UI
					NotifyPropertyChanged("IsRefreshEnabled");
					NotifyPropertyChanged("TrayIcon");
					NotifyPropertyChanged("TrayIconToolTip");
				}
			}
		}
		#endregion


		#region Additional GUI Items
		public System.Drawing.Icon TrayIcon
		{
			get
			{
				if(this.IsEnabled)
				{
					if(this.IsRefreshEnabled)
						return KeepMeBusy.Properties.Resources.Circle_Refresh;
					else
						return KeepMeBusy.Properties.Resources.Circle_Green;
				}
				else
				{
					return KeepMeBusy.Properties.Resources.Circle_Gray;
				}
			}
		}
		public string TrayIconToolTip
		{
			get
			{
				if (this.IsEnabled)
				{
					if (this.IsRefreshEnabled)
						return "Keep Me Busy: Active, refresh enabled";
					else
						return "Keep Me Busy: Active";
				}
				else
				{
					return "Keep Me Busy: Not Active";
				}
			}
		}
		public void RefreshUI()
		{
			NotifyPropertyChanged("AutoRunAtLogon");
			NotifyPropertyChanged("IsEnabled");
			NotifyPropertyChanged("IsRefreshEnabled");
			NotifyPropertyChanged("TrayIcon");
			NotifyPropertyChanged("TrayIconToolTip");
		}
		#endregion

		#region REGISTRY

		/// <summary>
		/// Allows properties that have a corresponding registry setting to be kept updated automatically.
		/// All property changes also result in Registry writes.
		/// </summary>
		public bool AutoSavePropertyValuesInRegistry { get; set; }

		public void LoadLastStateFromRegistry()
		{
			try
			{
				using (RegistryKey reg = Registry.CurrentUser.CreateSubKey(ConfigRegistryKey))
				{
					this._IsEnabled = Convert.ToBoolean((int)(reg.GetValue("IsEnabled", 0)));
					this._IsRefreshEnabled = Convert.ToBoolean((int)(reg.GetValue("IsRefreshEnabled", 0)));

					reg.Close();
				}
			}
			catch (Exception)
			{
				this._IsEnabled = false;
				this._IsRefreshEnabled = false;
			}
		}
		private void AutoSaveDWordSettingInRegistry(string settingName, int value)
		{
			if (AutoSavePropertyValuesInRegistry)
			{
				using (RegistryKey reg = Registry.CurrentUser.CreateSubKey(ConfigRegistryKey))
				{
					reg.SetValue(settingName, value, RegistryValueKind.DWord);
					reg.Close();
				}
			}
		}
		private bool SetAutoLoadAtLogonRegistry(bool AutoLoadEnabled)
		{
			try
			{
				if (AutoLoadEnabled)
				{
					using (RegistryKey reg = Registry.CurrentUser.CreateSubKey(CurrentUserAutoRunRegistry))
					{
						reg.SetValue("KeepMeBusy", this.AssemblyLocation, RegistryValueKind.String);
						reg.Close();
					}
				}
				else
				{
					using (RegistryKey reg = Registry.CurrentUser.CreateSubKey(CurrentUserAutoRunRegistry))
					{
						reg.DeleteValue("KeepMeBusy", false);
						reg.Close();
					}
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		private bool GetAutoLoadAtLogonRegistry()
		{
			string RegVal;
			bool result;

			try
			{
				// Check if the CurrentUser AutoRun Registry contains the KeepMeBusy entry 
				using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(CurrentUserAutoRunRegistry))
				{
					RegVal = (string)(reg.GetValue("KeepMeBusy", null));
					reg.Close();
				}
			}
			catch (Exception)
			{
				RegVal = null;
			}

			if(String.IsNullOrEmpty(RegVal))
			{
				result = false;	// AutoRun not enabled
			}
			else
			{
				if (RegVal.Equals(this.AssemblyLocation, StringComparison.OrdinalIgnoreCase))
					result = true;  // AutoRun is enabled and the path is correct
				else
					result = SetAutoLoadAtLogonRegistry(true);  // AutoRun is enabled but the app .exe file has probably been moved. Fix it.
			}

			this._AutoRunAtLogon = result;
			return result;
		}

		#endregion

	}
}
