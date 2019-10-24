using System;
using System.Diagnostics;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	public class WinAppDriverRunner : IDisposable
	{
		private readonly Process process = new Process();

		public WinAppDriverRunner()
		{
			process.StartInfo = new ProcessStartInfo
			{
				FileName = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
			};
		}

		public void Dispose()
		{
			process.CloseMainWindow();
			process.Dispose();
		}

		public void Run()
		{
			process.Start();
		}
	}
}