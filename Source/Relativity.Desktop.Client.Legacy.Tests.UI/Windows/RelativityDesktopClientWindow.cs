using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class RelativityDesktopClientWindow : WindowBase
	{
		public RelativityDesktopClientWindow(WindowDetails window)
			: base(window)
		{
		}

		public AppiumWebElement SelectRootFolder()
		{
			var rootFolder = GetRootFolder();
			rootFolder.Click();
			return rootFolder;
		}

		public AppiumWebElement GetRootFolder()
		{
			return Element.FindTreeWithAutomationId("_treeView").FindTree();
		}

		public void ClickDocumentLoadFileMenuItem()
		{
			Element.FindMenuBar("Application").ClickMenuItem("Tools").ClickMenuItem("Import")
				.ClickMenuItem("Document Load File...");
		}
	}
}