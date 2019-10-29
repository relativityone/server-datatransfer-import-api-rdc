using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class RelativityDesktopClientWindow : RdcWindowBase
	{
		private readonly MenuItemUIElement menuBar;
		private readonly UIElement treeView;

		public RelativityDesktopClientWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			menuBar = new MenuItemUIElement(FindMenuBar("Application"));
			treeView = FindTreeWithAutomationId("_treeView");
		}

		public void SelectRootFolder()
		{
			var rootFolder = treeView.FindTree();
			rootFolder.Click();
		}

		public ImportDocumentLoadFileWindow ImportDocumentLoadFile()
		{
			menuBar.ClickMenuItem("Tools").ClickMenuItem("Import")
				.ClickMenuItem("Document Load File...");
			return WindowsManager.SwitchToImportDocumentLoadFileWindow();
		}

		public ExportFolderAndSubfoldersWindow ExportFolderAndSubfolders()
		{
			menuBar.ClickMenuItem("Tools").ClickMenuItem("Export")
				.ClickMenuItem("Folder and Subfolders...");
			return WindowsManager.SwitchToExportFolderAndSubfoldersWindow();
		}
	}
}