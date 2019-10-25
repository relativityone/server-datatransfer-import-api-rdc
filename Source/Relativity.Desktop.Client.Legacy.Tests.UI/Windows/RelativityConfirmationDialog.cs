using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	public class RelativityConfirmationDialog : UIElement
	{
		private readonly UIElement okButton;

		public RelativityConfirmationDialog(Func<AppiumWebElement> create) : base(create)
		{
			okButton = FindButton("OK");
		}

		public void ClickOkButton()
		{
			okButton.Click();
		}
	}
}