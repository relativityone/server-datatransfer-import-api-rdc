using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class RelativityDesktopClientWindow : RdcWindowBase<RelativityDesktopClientWindow>
	{
		private readonly MenuItemUIElement menuBar;
		private readonly ComboBoxUIElement objectTypeComboBox;
		private readonly TreeUIElement treeView;

		public RelativityDesktopClientWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			menuBar = FindMenuBar("Application");
			treeView = FindTreeWithAutomationId("_treeView");
			objectTypeComboBox = FindComboBoxWithAutomationId("_objectTypeDropDown");
		}

		public void SelectRootFolder()
		{
			var rootFolder = treeView.FindTree();
			rootFolder.Click();
		}

		public RdoImportWindow ImportDocumentLoadFile()
		{
			ClickImportMenuItem("Document Load File...");
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
			menuBar.ClickMenuItem("Tools").ClickMenuItem("Export")
				.ClickMenuItem("Production Set...");
			return WindowsManager.SwitchToExportWindow(ExportProfile.ProductionSet);
		}

		public ExportWindow ExportImagingProfileObjects()
		{
			SelectObjectType("Imaging Profile");
			ClickExportMenuItem("Objects");
			return WindowsManager.SwitchToExportWindow(ExportProfile.ExportImagingProfileObjects);
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