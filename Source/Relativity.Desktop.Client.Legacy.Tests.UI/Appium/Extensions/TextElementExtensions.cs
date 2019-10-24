using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class TextElementExtensions
	{
		public static AppiumWebElement FindText(this AppiumWebElement element)
		{
			return element.FindChild(ElementType.Text);
		}

		public static AppiumWebElement FindTextWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.Text, automationId);
		}
	}
}