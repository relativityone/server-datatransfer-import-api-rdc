using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	public class SecurityAlertDialog : UIElement
	{
		private readonly UIElement yesButton;

		public SecurityAlertDialog(Func<AppiumWebElement> create) : base(create)
		{
			yesButton = FindButton("Yes");
		}

		public void ClickYesButton()
		{
			yesButton.Click();
		}
	}
}