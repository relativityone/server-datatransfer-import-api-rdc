using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class OpenSettingsDialog : UIElement<OpenSettingsDialog>
	{
		private readonly EditUIElement fileNameEdit;
		private readonly ButtonUIElement openButton;

		public OpenSettingsDialog(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			fileNameEdit = FindEdit("File name:");
			openButton = FindButtonWithClass("Open");
		}

		public void OpenSettingsFile(string settingsFile)
		{
			fileNameEdit.SendKeys(settingsFile);
			openButton.Click();
			//Waiting for dialog to be closed and settings loaded
			WaitToNotExist(TimeSpan.FromSeconds(5));
		}
	}
}