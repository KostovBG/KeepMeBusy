using System;
using System.Windows.Forms;

namespace KeepMeBusy
{
	static class Program
	{
		const string MutexGuid = "0a73297d-109e-431b-bf17-d6176dbd9cbd";

		[STAThread]
		static void Main()
		{
			System.Threading.Mutex mutex = new System.Threading.Mutex(false, MutexGuid);
			try
			{
				if (mutex.WaitOne(0, false))
				{
					using (BusySimulatorAppContext appContext = new BusySimulatorAppContext())
					{
						Application.Run(appContext);
					}
				}
			}
			finally
			{
				if (mutex != null)
				{
					mutex.Close();
					mutex = null;
				}
			}
		}
	}

	public class BusySimulatorAppContext : ApplicationContext
	{
		BusySimulatorUI BusySimUI;
		public BusySimulatorAppContext()
		{
			this.BusySimUI = new BusySimulatorUI();
		}

		private bool disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					this.BusySimUI.Dispose();
				}
				disposed = true;
			}
			base.Dispose(disposing);
		}
	}
}
