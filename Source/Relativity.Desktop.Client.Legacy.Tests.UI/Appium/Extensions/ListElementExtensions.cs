using System.Collections.Generic;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class ListElementExtensions
	{
		public static AppiumWebElement FindListWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.List, automationId);
		}

		public static IReadOnlyList<AppiumWebElement> FindListsWithAutomationId(this AppiumWebElement element,
			string automationId)
		{
			return element.FindChildrenWithAutomationId(ElementType.List, automationId);
		}

		public static IReadOnlyList<AppiumWebElement> FindListItems(this AppiumWebElement list)
		{
			return list.FindChildren(ElementType.ListItem);
		}

		public static AppiumWebElement FindListItemByIndex(this AppiumWebElement list, int index)
		{
			return list.FindElementByAccessibilityId($"ListViewItem-{index}");
		}

		public static AppiumWebElement FindListItemTextByIndex(this AppiumWebElement list, int index)
		{
			var listItem = list.FindListItemByIndex(index);
			return listItem.FindText();
		}
	}
}