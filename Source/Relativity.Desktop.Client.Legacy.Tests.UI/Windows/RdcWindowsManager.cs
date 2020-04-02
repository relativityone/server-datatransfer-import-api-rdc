using System;
using System.Collections.Generic;
using System.Linq;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class RdcWindowsManager
	{
		private readonly WindowsManager manager;
		private readonly RelativityDesktopClientWindow rdcWindow;
		private readonly Dictionary<string, IWindow> windows = new Dictionary<string, IWindow>();
		private readonly ILog logger;

		public RdcWindowsManager(ILog logger, WindowsManager manager)
		{
			this.logger = logger;
			this.manager = manager;
			rdcWindow = CreateRelativityDesktopClientWindow();
		}

		public LoginWindow SwitchToLoginWindow()
		{
			return SwitchToWindow(RdcWindowName.RelativityLogin, x => new LoginWindow(logger, this, x));
		}

		public bool TryGetUntrustedCertificateWindow(out UntrustedCertificateWindow window)
		{
			return TrySwitchToWindow(RdcWindowName.UntrustedCertificate, TimeSpan.FromSeconds(1),
				x => new UntrustedCertificateWindow(logger, this, x), out window);
		}

		public SelectWorkspaceWindow SwitchToSelectWorkspaceWindow()
		{
			return SwitchToWindow(RdcWindowName.SelectWorkspace, x => new SelectWorkspaceWindow(logger, this, x));
		}

		public RelativityDesktopClientWindow SwitchToRelativityDesktopClientWindow()
		{
			manager.SwitchToWindow(rdcWindow.Handle);
			return rdcWindow;
		}

		public RdoImportWindow SwitchToRdoImportWindow(RdoImportProfile profile)
		{
			return SwitchToWindow(profile.ImportWindow,x => new RdoImportWindow(logger, this, x, profile));
		}

		public ExportWindow SwitchToExportWindow(ExportProfile profile)
		{
			return SwitchToWindow(profile.ExportWindow, x => new ExportWindow(logger,this, x, profile));
		}

		public ImageImportWindow SwitchToImageImportWindow(ImageImportProfile profile)
		{
			return SwitchToWindow(profile.ImportWindow, x => new ImageImportWindow(logger, this, x, profile));
		}

		public DialogWindow GetRdcConfirmationDialog()
		{
			var windowDetails = GetWindow(RdcWindowName.RelativityDesktopClient,
				x => x.Handle != rdcWindow.Handle);
			return new DialogWindow(logger, this, windowDetails);
		}

		public bool TryGetRdcConfirmationDialog(out DialogWindow window)
		{
			return TrySwitchToWindow(RdcWindowName.RelativityDesktopClient,
				x => x.Handle != rdcWindow.Handle,
				TimeSpan.FromSeconds(3),
				x => new DialogWindow(logger, this, x), out window);
		}

		public ProgressWindow SwitchToProgressWindow(ProgressWindowName name)
		{
			return SwitchToWindow(name, x => new ProgressWindow(logger, this, x));
		}

		public void SwitchToWindow(string windowHandle)
		{
			manager.SwitchToWindow(windowHandle);
		}

		private RelativityDesktopClientWindow CreateRelativityDesktopClientWindow()
		{
			return SwitchToWindow(RdcWindowName.RelativityDesktopClient,
				x => new RelativityDesktopClientWindow(logger, this, x));
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

		private bool TrySwitchToWindow<T>(RdcWindowName name, Func<WindowDetails, bool> predicate, TimeSpan waitForWindowTimeout,
			Func<WindowDetails, T> createWindow, out T window)
			where T : RdcWindowBase<T>
		{
			ClearClosedWindows();

			if (!manager.TryGetWindow(name, predicate, waitForWindowTimeout, out var windowDetails))
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

		private bool TrySwitchToWindow<T>(RdcWindowName name, TimeSpan waitForWindowTimeout,
			Func<WindowDetails, T> createWindow, out T window)
			where T : RdcWindowBase<T>
		{
			return TrySwitchToWindow(name, _ => true, waitForWindowTimeout, createWindow, out window);
		}

		private void ClearClosedWindows()
		{
			var openedWindows = manager.OpenedWindowsHandles;
			windows.Keys.Where(x => openedWindows.All(handle => handle != x)).ToList().ForEach(x => windows.Remove(x));
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