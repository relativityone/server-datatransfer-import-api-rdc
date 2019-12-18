using System;
using System.Threading;
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
			ClickImportMenuItem("Document Load File...");
			Thread.Sleep(5000);
			return WindowsManager.SwitchToRdoImportWindow(RdoImportProfile.DocumentLoadFile);
		}

		public ImageImportWindow ImportProductionLoadFile()
		{
			ClickImportMenuItem("Production Load File...");
			return WindowsManager.SwitchToImageImportWindow(ImageImportProfile.ProductionLoadFile);
		}

		public ImageImportWindow ImportImageLoadFile()
		{
			ClickImportMenuItem("Image Load File...");
			return WindowsManager.SwitchToImageImportWindow(ImageImportProfile.ImageLoadFile);
		}

		public RdoImportWindow ImportImagingProfileObjects()
		{
			SelectObjectType("Imaging Profile");
			ClickImportMenuItem("Imaging Profile Load File...");
			return WindowsManager.SwitchToRdoImportWindow(RdoImportProfile.ImagingProfileLoadFile);
		}

		public ExportWindow ExportFolderAndSubfolders()
		{
			ClickExportMenuItem("Folder and Subfolders...");
			return WindowsManager.SwitchToExportWindow(ExportProfile.FoldersAndSubfolders);
		}

		public ExportWindow ExportSavedSearch()
		{
			ClickExportMenuItem("Saved Search...");
			return WindowsManager.SwitchToExportWindow(ExportProfile.ExportSavedSearch);
		}

		public ExportWindow ExportProductionSet()
		{
			ClickExportMenuItem("Production Set...");
			return WindowsManager.SwitchToExportWindow(ExportProfile.ProductionSet);
		}

		public ExportWindow ExportImagingProfileObjects()
		{
			SelectObjectType("Imaging Profile");
			ClickExportMenuItem("Objects");
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

		private void ClickExportMenuItem(string name)
		{
			menuBar.ClickMenuItem("Tools").ClickMenuItem("Export").ClickMenuItem(name);
		}

		private void ClickImportMenuItem(string name)
		{
			menuBar.ClickMenuItem("Tools").ClickMenuItem("Import").ClickMenuItem(name);
		}
	}
}