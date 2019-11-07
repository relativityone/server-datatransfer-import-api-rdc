using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ButtonUIElement : UIElement<ButtonUIElement>
	{
		public ButtonUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}
	}
}