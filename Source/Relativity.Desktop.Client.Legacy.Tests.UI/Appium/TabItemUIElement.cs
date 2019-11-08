using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class TabItemUIElement : UIElement<TabItemUIElement>
	{
		public TabItemUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}
	}
}