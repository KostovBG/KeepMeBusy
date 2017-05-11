using System;
using System.Runtime.InteropServices;

namespace KeepMeBusy
{
	//
	// https://msdn.microsoft.com/en-us/library/aa373208(VS.85).aspx
	//

	public static class ThreadStateController
	{
		[Flags]
		private enum EXECUTION_STATE : uint
		{
			ES_AWAYMODE_REQUIRED = 0x00000040,
			ES_CONTINUOUS = 0x80000000,
			ES_DISPLAY_REQUIRED = 0x00000002,
			ES_SYSTEM_REQUIRED = 0x00000001
			// Legacy flag, should not be used.
			// ES_USER_PRESENT = 0x00000004
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

		[DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("USER32.DLL")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "SystemParametersInfo")]
		internal static extern int SystemParametersInfo(int uiAction, int uiParam, ref int pvParam, int fWinIni);

		public static bool SetBusy()
		{
			var res = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
			return (res != 0);
		}

		public static bool SetDefault()
		{
			var res = SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
			return (res != 0);
		}

	}
}
