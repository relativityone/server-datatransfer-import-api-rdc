﻿using System;
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
		private readonly Dictionary<string, IWindow> windows = new Dictionary<string, IWindow>();

		public RdcWindowsManager(WindowsManager manager)
		{
			this.manager = manager;
			rdcWindow = CreateRelativityDesktopClientWindow();
		}

		public LoginWindow SwitchToLoginWindow()
		{
			return SwitchToWindow(WindowNameConstants.RelativityLogin, x => new LoginWindow(this, x));
		}

		public bool TryGetUntrustedCertificateWindow(out UntrustedCertificateWindow window)
		{
			return TrySwitchToWindow(WindowNameConstants.UntrustedCertificate, TimeSpan.FromSeconds(1),
				x => new UntrustedCertificateWindow(this, x), out window);
		}

		public SelectWorkspaceWindow SwitchToSelectWorkspaceWindow()
		{
			return SwitchToWindow(WindowNameConstants.SelectWorkspace, x => new SelectWorkspaceWindow(this, x));
		}

		public RelativityDesktopClientWindow SwitchToRelativityDesktopClientWindow()
		{
			manager.SwitchToWindow(rdcWindow.Handle);
			return rdcWindow;
		}

		public ImportDocumentLoadFileWindow SwitchToImportDocumentLoadFileWindow()
		{
			return SwitchToWindow(WindowNameConstants.ImportDocumentLoadFile,
				x => new ImportDocumentLoadFileWindow(this, x));
		}

		public ProgressWindow SwitchToImportLoadFileProgressWindow()
		{
			return SwitchToProgressWindow(WindowNameConstants.ImportLoadFileProgress);
		}

		public ProgressWindow SwitchToExportFoldersAndSubfoldersProgress()
		{
			return SwitchToProgressWindow(WindowNameConstants.ExportFoldersAndSubfoldersProgress);
		}

		public ExportFolderAndSubfoldersWindow SwitchToExportFolderAndSubfoldersWindow()
		{
			return SwitchToWindow(WindowNameConstants.ExportFolderAndSubfolders,
				x => new ExportFolderAndSubfoldersWindow(this, x));
		}

		public DialogWindow GetRdcConfirmationDialog()
		{
			var windowDetails = GetWindow(WindowNameConstants.RelativityDesktopClient,
				x => x.Handle != rdcWindow.Handle);
			return new DialogWindow(this, windowDetails);
		}

		private ProgressWindow SwitchToProgressWindow(string title)
		{
			return SwitchToWindow(title, x => new ProgressWindow(this, x));
		}

		private RelativityDesktopClientWindow CreateRelativityDesktopClientWindow()
		{
			return SwitchToWindow(WindowNameConstants.RelativityDesktopClient,
				x => new RelativityDesktopClientWindow(this, x));
		}

		private T SwitchToWindow<T>(string title, Func<WindowDetails, T> createWindow) where T : RdcWindowBase<T>
		{
			if (!TrySwitchToWindow(title, createWindow, out var window))
			{
				throw new Exception($"'{title}' window was not found.");
			}

			return window;
		}

		private bool TrySwitchToWindow<T>(string title, Func<WindowDetails, T> createWindow, out T window)
			where T : RdcWindowBase<T>
		{
			return TrySwitchToWindow(title, DefaultTimeouts.WaitForWindow, createWindow, out window);
		}

		private bool TrySwitchToWindow<T>(string title, TimeSpan waitForWindowTimeout,
			Func<WindowDetails, T> createWindow, out T window)
			where T : RdcWindowBase<T>
		{
			ClearClosedWindows();

			if (!manager.TryGetWindow(title, waitForWindowTimeout, out var windowDetails))
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

		private WindowDetails GetWindow(string title, Func<WindowDetails, bool> predicate)
		{
			if (!manager.TryGetWindow(title, predicate, DefaultTimeouts.WaitForWindow, out var window))
			{
				throw new Exception($"'{title}' window was not found.");
			}

			return window;
		}
	}
}