using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ListItemUIElement : UIElement<ListItemUIElement>
	{
		private readonly TextUIElement textBox;

		public ListItemUIElement(Func<AppiumWebElement> create) : base(create)
		{
			textBox = FindText();
		}

		public override void Click()
		{
			if (textBox.Exists)
			{
				textBox.Click();
			}
			else
			{
				base.Click();
			}
		}
	}
}