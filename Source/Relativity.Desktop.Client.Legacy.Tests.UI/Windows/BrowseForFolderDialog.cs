using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class BrowseForFolderDialog : UIElement<BrowseForFolderDialog>
	{
		private readonly ButtonUIElement cancelButton;

		public BrowseForFolderDialog(Func<AppiumWebElement> create) : base(create)
		{
			cancelButton = FindButton("Cancel");
		}

		public void ClickCancelButton()
		{
			cancelButton.Click();
		}
	}
}