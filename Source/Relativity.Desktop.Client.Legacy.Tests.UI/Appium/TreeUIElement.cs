using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class TreeUIElement : UIElement<TreeUIElement>
	{
		public TreeUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}
	}
}