using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class EditUIElement : UIElement
	{
		public EditUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public void SetText(string text)
		{
			Clear();
			Element.SendKeys(text);
		}

		public void Clear()
		{
			while (!string.IsNullOrEmpty(Text))
			{
				Element.SendKeys(Keys.Backspace);
				Element.SendKeys(Keys.Delete);
			}
		}
	}
}