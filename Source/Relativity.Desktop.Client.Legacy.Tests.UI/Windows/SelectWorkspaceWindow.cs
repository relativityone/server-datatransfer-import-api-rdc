namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	using OpenQA.Selenium.Appium;
	using OpenQA.Selenium.Appium.Windows;

	using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

	internal class SelectWorkspaceWindow
	{
		public SelectWorkspaceWindow(WindowsElement window)
		{
			this.Window = window;
		}

		public WindowsElement Window { get; }

		public void EnterSearchText(string searchText)
		{
			var searchTextBox = this.GetSearchTextBox();
			searchTextBox.SendKeys(searchText);
		}

		public void ClickWorkspace(int index)
		{
			var list = this.Window.FindListWithAutomationId("ItemListView");
			var listItemText = list.FindListItemTextByIndex(index);
			listItemText.Click();
		}

		public void ClickOkButton()
		{
			this.Window.ClickButtonWithAutomationId("OKButton");
		}

		private AppiumWebElement GetSearchTextBox()
		{
			return this.Window.FindEditWithAutomationId("SearchQuery");
		}
	}
}