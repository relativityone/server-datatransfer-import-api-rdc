using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class WindowElementExtensions
	{
		public static AppiumWebElement FindWindow(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.Window, name);
		}

		public static AppiumWebElement WaitForWindow(this AppiumWebElement element, string name)
		{
			return element.WaitForChild(x => x.FindChildren(ElementType.Window, name));
		}
	}
}