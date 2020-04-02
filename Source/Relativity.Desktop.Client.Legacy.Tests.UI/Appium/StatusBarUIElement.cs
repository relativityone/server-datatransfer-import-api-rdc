using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class StatusBarUIElement : UIElement<StatusBarUIElement>
	{
		public StatusBarUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
		}
	}
}