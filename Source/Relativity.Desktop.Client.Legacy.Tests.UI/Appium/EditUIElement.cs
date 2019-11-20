using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class EditUIElement : UIElement<EditUIElement>
	{
		public EditUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public void SetText(string text)
		{
			Clear();
			SendKeys(text);
		}

		public void Clear()
		{
			while (!string.IsNullOrEmpty(Text))
			{
				SendKeys(Keys.Backspace);
				SendKeys(Keys.Delete);
			}
		}
	}
}