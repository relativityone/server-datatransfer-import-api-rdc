using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	public class WinAppDriverRunner : IDisposable
	{
		private const string WinAppDriverExePath =
			@"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";

		private const string WinAppDriverProcessName = "WinAppDriver";

		private Process process;

		public void Dispose()
		{
			if (process != null)
			{
				process.CloseMainWindow();
				process.Dispose();
			}
		}

		private bool IsAlreadyRunning()
		{
			return Process.GetProcesses().Any(x => string.Equals(x.ProcessName, WinAppDriverProcessName,
				StringComparison.InvariantCultureIgnoreCase));
		}

		public void Run()
		{
			if (IsAlreadyRunning())
			{
				return;
			}

			EnsureWinAppDriverIsInstalled();

			process = new Process {StartInfo = new ProcessStartInfo {FileName = WinAppDriverExePath}};
			process.Start();
		}

		private static void EnsureWinAppDriverIsInstalled()
		{
			if (!File.Exists(WinAppDriverExePath))
			{
				throw new Exception(
					$"WinAppDriver is not installed. The service executable cannot be found at {WinAppDriverExePath}");
			}
		}
	}
}