using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class ImportDocumentLoadFileWindow : WindowBase
	{
		public ImportDocumentLoadFileWindow(WindowDetails window)
			: base(window)
		{
		}

		public void ClickLoadImportSettingsMenuItem()
		{
			Element.FindMenuBar("Application").ClickMenuItem("File")
				.ClickMenuItem("Load Import Settings (kwe)\tCtrl+O");
		}

		public void ClickImportFileMenuItem()
		{
			Element.FindMenuBar("Application").ClickMenuItem("Import").ClickMenuItem("Import File...");
		}

		private AppiumWebElement GetOpenSavedFieldMapDialog()
		{
			return Element.WaitForWindow("Open Saved Field Map");
		}

		public void LoadKweFile(string kweFile, string dateFile)
		{
			var openDialog = GetOpenSavedFieldMapDialog();
			openDialog.FindEdit("File name:").SendKeys(kweFile);
			openDialog.ClickButtonWithClass("Open", ElementClass.Button);
			openDialog.WaitForWindow("Relativity.Desktop.Client").ClickButton("OK");

			var loadFileDialog = openDialog.WaitForWindow("Choose Load File");
			loadFileDialog.FindEdit("File name:").SendKeys(dateFile);
			loadFileDialog.ClickButtonWithClass("Open", ElementClass.Button);
		}
	}
}