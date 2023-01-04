using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class BrowseForFolderDialog : UIElement<BrowseForFolderDialog>
	{
		private readonly ButtonUIElement cancelButton;

		public BrowseForFolderDialog(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			cancelButton = FindButton("Cancel");
		}

		public void ClickCancelButton()
		{
			cancelButton.Click();
		}
	}
}