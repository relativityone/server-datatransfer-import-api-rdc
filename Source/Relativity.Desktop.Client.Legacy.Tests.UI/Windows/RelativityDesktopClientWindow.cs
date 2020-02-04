using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class RelativityDesktopClientWindow : RdcWindowBase<RelativityDesktopClientWindow>
	{
		private readonly MenuItemUIElement menuBar;
		private readonly ComboBoxUIElement objectTypeComboBox;
		private readonly TreeUIElement treeView;
		private readonly TextUIElement statusBarText;

		private static readonly Dictionary<ImportMenuItems, string> ImportMenuItemsCollection = new Dictionary<ImportMenuItems, string>()
		{
			[ImportMenuItems.DocumentLoadFile] = Keys.LeftControl + "L",
			[ImportMenuItems.ImageLoadFile] = Keys.LeftControl + "I",
			[ImportMenuItems.ImagingProfileLoadFile] = Keys.LeftControl + "L",
			[ImportMenuItems.ProductionLoadFile] = Keys.LeftControl + "P"
		};
		
		private static readonly Dictionary<ExportMenuItems, string> ExportMenuItemsCollection = new Dictionary<ExportMenuItems, string>()
		{
			[ExportMenuItems.Folder] = Keys.LeftControl + "F",
			[ExportMenuItems.FolderAndSubfolders] = Keys.LeftControl + "FF",
			[ExportMenuItems.Objects] = Keys.LeftControl + "O",
			[ExportMenuItems.ProductionSet] = Keys.LeftControl + "P",
			[ExportMenuItems.SavedSearch] = Keys.LeftControl + "S"
		};

		public RelativityDesktopClientWindow(ILog logger, RdcWindowsManager windowsManager, WindowDetails window)
			: base(logger, windowsManager, window)
		{
			menuBar = FindMenuBar("Application");
			treeView = FindTreeWithAutomationId("_treeView");
			objectTypeComboBox = FindComboBoxWithAutomationId("_objectTypeDropDown");
			statusBarText = FindStatusBarWithAutomationId("StatusBar").FindText();
		}

		public void SelectRootFolder()
		{
			var rootFolder = treeView.FindTreeItem().WaitFor(TimeSpan.FromSeconds(10));
			rootFolder.Click();
			rootFolder.WaitToBeSelected(TimeSpan.FromSeconds(2));
		}

		public RdoImportWindow ImportDocumentLoadFile()
		{
			SelectImportMenuItem(ImportMenuItems.DocumentLoadFile);
			Thread.Sleep(5000);
			return WindowsManager.SwitchToRdoImportWindow(RdoImportProfile.DocumentLoadFile);
		}

		public ImageImportWindow ImportProductionLoadFile()
		{
			SelectImportMenuItem(ImportMenuItems.ProductionLoadFile);
			return WindowsManager.SwitchToImageImportWindow(ImageImportProfile.ProductionLoadFile);
		}

		public ImageImportWindow ImportImageLoadFile()
		{
			SelectImportMenuItem(ImportMenuItems.ImageLoadFile);
			return WindowsManager.SwitchToImageImportWindow(ImageImportProfile.ImageLoadFile);
		}

		public RdoImportWindow ImportImagingProfileObjects()
		{
			SelectObjectType("Imaging Profile");
			SelectImportMenuItem(ImportMenuItems.ImagingProfileLoadFile);
			return WindowsManager.SwitchToRdoImportWindow(RdoImportProfile.ImagingProfileLoadFile);
		}

		public ExportWindow ExportFolderAndSubfolders()
		{
			SelectExportMenuItem(ExportMenuItems.FolderAndSubfolders);
			return WindowsManager.SwitchToExportWindow(ExportProfile.FoldersAndSubfolders);
		}

		public ExportWindow ExportSavedSearch()
		{
			SelectExportMenuItem(ExportMenuItems.SavedSearch);
			return WindowsManager.SwitchToExportWindow(ExportProfile.ExportSavedSearch);
		}

		public ExportWindow ExportProductionSet()
		{
			SelectExportMenuItem(ExportMenuItems.ProductionSet);
			return WindowsManager.SwitchToExportWindow(ExportProfile.ProductionSet);
		}

		public ExportWindow ExportImagingProfileObjects()
		{
			SelectObjectType("Imaging Profile");
			SelectExportMenuItem(ExportMenuItems.Objects);
			return WindowsManager.SwitchToExportWindow(ExportProfile.ExportImagingProfileObjects);
		}

		public void WaitForTransferModeDetection()
		{
			Wait.For(() => statusBarText.Text != "Workspace Loaded - File Transfer Mode: Connecting...",
				TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(3));
		}

		private void SelectObjectType(string objectTypeName)
		{
			objectTypeComboBox.SelectComboBoxItem(objectTypeName);
		}

		private void SelectExportMenuItem(ExportMenuItems name)
		{
			Logger.LogDebug($"Select Export menu option '{name}'");
			menuBar.SendKeys(Keys.Alt + "T");
			menuBar.SendKeys("E");
			menuBar.SendKeys(ExportMenuItemsCollection[name]);
			menuBar.SendKeys(Keys.Enter);
		}

		private void SelectImportMenuItem(ImportMenuItems name)
		{
			Logger.LogDebug($"Select Import menu option '{name}'");
			menuBar.SendKeys(Keys.Alt + "T");
			menuBar.SendKeys("I");
			menuBar.SendKeys(ImportMenuItemsCollection[name]);
			menuBar.SendKeys(Keys.Enter);
		}
	}
}