namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	using OpenQA.Selenium.Appium;
	using OpenQA.Selenium.Appium.Windows;

	using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

	public class ImportDocumentLoadFileWindow
	{
		private readonly WindowsElement window;

		public ImportDocumentLoadFileWindow(WindowsElement window)
		{
			this.window = window;
		}

		public void ClickLoadImportSettingsMenuItem()
		{
			this.window.FindMenuBar("Application").ClickMenuItem("File").ClickMenuItem("Load Import Settings (kwe)\tCtrl+O");
		}

		public void ClickImportFileMenuItem()
		{
			this.window.FindMenuBar("Application").ClickMenuItem("Import").ClickMenuItem("Import File...");
		}

		private AppiumWebElement GetOpenSavedFieldMapDialog()
		{
			return this.window.FindWindow("Open Saved Field Map");
		}

		public void LoadKweFile(string kweFile, string dateFile)
		{
			var openDialog = this.GetOpenSavedFieldMapDialog();
			openDialog.FindEdit("File name:").SendKeys(kweFile);
			Wait.Tiny();

			openDialog.ClickButtonWithClass("Open", ElementClass.Button);
			Wait.Small();

			openDialog.FindWindow("Relativity.Desktop.Client").ClickButton("OK");
			Wait.Second();

			var loadFileDialog = openDialog.FindElementByName("Choose Load File");
			loadFileDialog.FindEdit("File name:").SendKeys(dateFile);
			loadFileDialog.ClickButtonWithClass("Open", ElementClass.Button);
		}
	}
}