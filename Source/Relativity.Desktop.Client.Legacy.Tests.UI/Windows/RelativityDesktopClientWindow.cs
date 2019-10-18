namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	using OpenQA.Selenium.Appium;
	using OpenQA.Selenium.Appium.Windows;

	using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

	public class RelativityDesktopClientWindow
	{
		private readonly WindowsElement window;

		public RelativityDesktopClientWindow(WindowsElement window)
		{
			this.window = window;
		}

		public AppiumWebElement GetRootFolder()
		{
			return this.window.FindTreeWithAutomationId("_treeView").FindTree();
		}

		public void ClickDocumentLoadFileMenuItem()
		{
			this.window.FindMenuBar("Application")
				.ClickMenuItem("Tools")
				.ClickMenuItem("Import")
				.ClickMenuItem("Document Load File...");
		}
	}
}