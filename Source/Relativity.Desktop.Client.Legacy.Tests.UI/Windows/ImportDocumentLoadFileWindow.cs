using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class ImportDocumentLoadFileWindow : RdcWindowBase
	{
		private readonly MenuBarUIElement menuBar;
		private readonly OpenSavedFieldMapDialog openSavedFieldMapDialog;

		public ImportDocumentLoadFileWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			menuBar = new MenuBarUIElement(FindMenuBar("Application"));
			openSavedFieldMapDialog = new OpenSavedFieldMapDialog(WaitForWindow("Open Saved Field Map"));
		}

		public void LoadImportSettings()
		{
			menuBar.ClickMenuItem("File").ClickMenuItem("Load Import Settings (kwe)\tCtrl+O");
		}

		public void ClickImportFileMenuItem()
		{
			menuBar.ClickMenuItem("Import").ClickMenuItem("Import File...");
		}

		public void LoadKweFile(string kweFile, string datFile)
		{
			openSavedFieldMapDialog.LoadKweFile(kweFile, datFile);
		}
	}
}