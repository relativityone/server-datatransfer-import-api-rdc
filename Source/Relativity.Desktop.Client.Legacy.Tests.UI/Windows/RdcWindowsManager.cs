using System;
using System.Collections.Generic;
using System.Linq;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class RdcWindowsManager
	{
		private readonly WindowsManager manager;
		private readonly RelativityDesktopClientWindow rdcWindow;
		private readonly Dictionary<string, IWindow> windows = new Dictionary<string, IWindow>();

		public RdcWindowsManager(WindowsManager manager)
		{
			this.manager = manager;
			rdcWindow = CreateRelativityDesktopClientWindow();
		}

		public LoginWindow SwitchToLoginWindow()
		{
			return SwitchToWindow(RdcWindowName.RelativityLogin, x => new LoginWindow(this, x));
		}

		public bool TryGetUntrustedCertificateWindow(out UntrustedCertificateWindow window)
		{
			return TrySwitchToWindow(RdcWindowName.UntrustedCertificate, TimeSpan.FromSeconds(1),
				x => new UntrustedCertificateWindow(this, x), out window);
		}

		public SelectWorkspaceWindow SwitchToSelectWorkspaceWindow()
		{
			return SwitchToWindow(RdcWindowName.SelectWorkspace, x => new SelectWorkspaceWindow(this, x));
		}

		public RelativityDesktopClientWindow SwitchToRelativityDesktopClientWindow()
		{
			manager.SwitchToWindow(rdcWindow.Handle);
			return rdcWindow;
		}

		public RdoImportWindow SwitchToRdoImportWindow(RdoImportProfile profile)
		{
			return SwitchToWindow(profile.ImportWindow,x => new RdoImportWindow(this, x, profile));
		}

		public ExportWindow SwitchToExportWindow(ExportProfile profile)
		{
			return SwitchToWindow(profile.ExportWindow, x => new ExportWindow(this, x, profile));
		}

		public ImageImportWindow SwitchToImageImportWindow(ImageImportProfile profile)
		{
			return SwitchToWindow(profile.ImportWindow, x => new ImageImportWindow(this, x, profile));
		}

		public DialogWindow GetRdcConfirmationDialog()
		{
			var windowDetails = GetWindow(RdcWindowName.RelativityDesktopClient,
				x => x.Handle != rdcWindow.Handle);
			return new DialogWindow(this, windowDetails);
		}

		public ProgressWindow SwitchToProgressWindow(ProgressWindowName name)
		{
			return SwitchToWindow(name, x => new ProgressWindow(this, x));
		}

		private RelativityDesktopClientWindow CreateRelativityDesktopClientWindow()
		{
			return SwitchToWindow(RdcWindowName.RelativityDesktopClient,
				x => new RelativityDesktopClientWindow(this, x));
		}

		private T SwitchToWindow<T>(RdcWindowName name, Func<WindowDetails, T> createWindow) where T : RdcWindowBase<T>
		{
			if (!TrySwitchToWindow(name, createWindow, out var window))
			{
				throw new Exception($"'{name}' window was not found.");
			}

			return window;
		}

		private bool TrySwitchToWindow<T>(RdcWindowName name, Func<WindowDetails, T> createWindow, out T window)
			where T : RdcWindowBase<T>
		{
			return TrySwitchToWindow(name, DefaultTimeouts.WaitForWindow, createWindow, out window);
		}

		private bool TrySwitchToWindow<T>(RdcWindowName name, TimeSpan waitForWindowTimeout,
			Func<WindowDetails, T> createWindow, out T window)
			where T : RdcWindowBase<T>
		{
			ClearClosedWindows();

			if (!manager.TryGetWindow(name, waitForWindowTimeout, out var windowDetails))
			{
				window = null;
				return false;
			}

			if (windows.TryGetValue(windowDetails.Handle, out var windowBase))
			{
				window = (T) windowBase;
			}
			else
			{
				window = createWindow(windowDetails);
				windows[window.Handle] = window;
			}

			return true;
		}

		private void ClearClosedWindows()
		{
			windows.Keys.Where(x => !manager.IsOpen(x)).ToList().ForEach(x => windows.Remove(x));
		}

		private WindowDetails GetWindow(RdcWindowName name, Func<WindowDetails, bool> predicate)
		{
			if (!manager.TryGetWindow(name, predicate, DefaultTimeouts.WaitForWindow, out var window))
			{
				throw new Exception($"'{name}' window was not found.");
			}

			return window;
		}
	}
}