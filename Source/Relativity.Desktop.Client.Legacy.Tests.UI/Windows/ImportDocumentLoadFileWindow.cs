using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class ImportDocumentLoadFileWindow : RdcWindowBase
	{
		private readonly MenuItemUIElement menuBar;
		private readonly OpenSavedFieldMapDialog openSavedFieldMapDialog;

		public ImportDocumentLoadFileWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			menuBar = new MenuItemUIElement(FindMenuBar("Application"));
			openSavedFieldMapDialog = new OpenSavedFieldMapDialog(WaitForWindow("Open Saved Field Map"));
		}

		public void ClickImportFileMenuItem()
		{
			menuBar.ClickMenuItem("Import").ClickMenuItem("Import File...");
		}

		public void LoadKweFile(string kweFile, string datFile)
		{
			menuBar.ClickMenuItem("File").ClickMenuItem("Load Import Settings (kwe)\tCtrl+O");
			openSavedFieldMapDialog.LoadKweFile(kweFile, datFile);
		}
	}
}