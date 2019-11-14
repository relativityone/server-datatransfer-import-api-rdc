using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class TextUIElement : UIElement<TextUIElement>
	{
		public TextUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}
	}
}