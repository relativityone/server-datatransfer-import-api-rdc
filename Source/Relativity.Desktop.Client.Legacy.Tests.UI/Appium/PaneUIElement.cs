using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class PaneUIElement : UIElement<PaneUIElement>
	{
		public PaneUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}
	}
}