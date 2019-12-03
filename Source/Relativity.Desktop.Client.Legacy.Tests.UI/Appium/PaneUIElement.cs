using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class PaneUIElement : UIElement<PaneUIElement>
	{
		public PaneUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
		}
	}
}