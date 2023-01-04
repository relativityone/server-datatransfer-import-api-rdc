using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class EditUIElement : UIElement<EditUIElement>
	{
		public EditUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
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