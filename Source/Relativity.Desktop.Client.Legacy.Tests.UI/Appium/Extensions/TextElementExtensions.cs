using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class TextElementExtensions
	{
		public static AppiumWebElement FindText(this AppiumWebElement element)
		{
			return element.FindChild(ElementType.Text);
		}
	}
}