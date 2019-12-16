using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ButtonUIElement : UIElement<ButtonUIElement>
	{
		public ButtonUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
		}
	}
}