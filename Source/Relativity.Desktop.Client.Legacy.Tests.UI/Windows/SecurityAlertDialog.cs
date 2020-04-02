using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class SecurityAlertDialog : UIElement<SecurityAlertDialog>
	{
		private readonly ButtonUIElement yesButton;

		public SecurityAlertDialog(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			yesButton = FindButton("Yes");
		}

		public void ClickYesButton()
		{
			yesButton.Click();
		}
	}
}