using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class RelativityDesktopClientWindow : RdcWindowBase
	{
		private readonly MenuBarUIElement menuBar;
		private readonly UIElement treeView;

		public RelativityDesktopClientWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			menuBar = new MenuBarUIElement(FindMenuBar("Application"));
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
	}
}