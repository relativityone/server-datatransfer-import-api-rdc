namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	using OpenQA.Selenium.Appium;

	public static class ListElementExtensions
	{
		public static AppiumWebElement FindListItem(this AppiumWebElement list, string name)
		{
			return list.FindChild(ElementType.ListItem, name);
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