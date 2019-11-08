using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class GroupUIElement : UIElement<GroupUIElement>
	{
		public GroupUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}
	}
}