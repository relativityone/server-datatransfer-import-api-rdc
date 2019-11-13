using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class OpenSettingsDialog : UIElement<OpenSettingsDialog>
	{
		private readonly EditUIElement fileNameEdit;
		private readonly ButtonUIElement openButton;

		public OpenSettingsDialog(Func<AppiumWebElement> create) : base(create)
		{
			fileNameEdit = FindEdit("File name:");
			openButton = FindButtonWithClass("Open");
		}

		public void OpenSettingsFile(string settingsFile)
		{
			fileNameEdit.SendKeys(settingsFile);
			openButton.Click();
		}
	}
}