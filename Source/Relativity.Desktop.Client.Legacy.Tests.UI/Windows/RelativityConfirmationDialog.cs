using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class RelativityConfirmationDialog : UIElement<RelativityConfirmationDialog>
	{
		private readonly ButtonUIElement okButton;

		public RelativityConfirmationDialog(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			okButton = FindButton("OK");
		}

		public void ClickOkButton()
		{
			okButton.Click();
		}
	}
}