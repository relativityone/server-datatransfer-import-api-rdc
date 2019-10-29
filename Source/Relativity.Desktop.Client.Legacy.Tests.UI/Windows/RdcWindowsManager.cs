using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class RdcWindowsManager
	{
		private readonly WindowsManager manager;
		private readonly RelativityDesktopClientWindow rdcWindow;
		private readonly Dictionary<string, WindowBase> windows = new Dictionary<string, WindowBase>();

		public RdcWindowsManager(WindowsManager manager)
		{
			this.manager = manager;
			rdcWindow = CreateRelativityDesktopClientWindow();
		}

		public LoginWindow SwitchToLoginWindow()
		{
			return SwitchToWindow(WindowNames.RelativityLogin, x => new LoginWindow(this, x));
		}

		public UntrustedCertificateWindow SwitchToUntrustedCertificateWindow()
		{
			return SwitchToWindow(WindowNames.UntrustedCertificate, x => new UntrustedCertificateWindow(this, x));
		}

		public SelectWorkspaceWindow SwitchToSelectWorkspaceWindow()
		{
			return SwitchToWindow(WindowNames.SelectWorkspace, x => new SelectWorkspaceWindow(this, x));
		}

		public RelativityDesktopClientWindow SwitchToRelativityDesktopClientWindow()
		{
			manager.SwitchToWindow(rdcWindow.Handle);
			return rdcWindow;
		}

		public ImportDocumentLoadFileWindow SwitchToImportDocumentLoadFileWindow()
		{
			return SwitchToWindow(WindowNames.ImportDocumentLoadFile, x => new ImportDocumentLoadFileWindow(this, x));
		}

		public ProgressWindow SwitchToImportLoadFileProgressWindow()
		{
			return SwitchToProgressWindow(WindowNames.ImportLoadFileProgress);
		}

		public ProgressWindow SwitchToExportFoldersAndSubfoldersProgress()
		{
			return SwitchToProgressWindow(WindowNames.ExportFoldersAndSubfoldersProgress);
		}

		public ExportFolderAndSubfoldersWindow SwitchToExportFolderAndSubfoldersWindow()
		{
			return SwitchToWindow(WindowNames.ExportFolderAndSubfolders,
				x => new ExportFolderAndSubfoldersWindow(this, x));
		}

		public AppiumWebElement GetRdcConfirmationDialog()
		{
			return manager.GetWindow(WindowNames.RelativityDesktopClient, x => x.Handle != rdcWindow.Handle)
				.Element;
		}

		private ProgressWindow SwitchToProgressWindow(string title)
		{
			return SwitchToWindow(title, x => new ProgressWindow(this, x));
		}

		private RelativityDesktopClientWindow CreateRelativityDesktopClientWindow()
		{
			var windowDetails = GetWindow(WindowNames.RelativityDesktopClient);
			var window = new RelativityDesktopClientWindow(this, windowDetails);
			windows.Add(windowDetails.Handle, window);
			return window;
		}

		private T SwitchToWindow<T>(string title, Func<WindowDetails, T> createWindow) where T : RdcWindowBase
		{
			ClearClosedWindows();

			var windowDetails = GetWindow(title);

			if (windows.TryGetValue(windowDetails.Handle, out var windowBase)) return (T) windowBase;

			var window = createWindow(windowDetails);
			windows[window.Handle] = window;
			return window;
		}

		private void ClearClosedWindows()
		{
			windows.Keys.Where(x => !manager.IsOpen(x)).ToList().ForEach(x => windows.Remove(x));
		}

		private WindowDetails GetWindow(string title)
		{
			return manager.GetWindow(title);
		}
	}
}