using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ListItemUIElement : UIElement<ListItemUIElement>
	{
		private readonly TextUIElement textBox;

		public ListItemUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			textBox = FindText();
		}

		public override void Click()
		{
			base.Click();
			if (!Selected && textBox.Exists)
			{
				textBox.Click();
			}
		}
	}
}