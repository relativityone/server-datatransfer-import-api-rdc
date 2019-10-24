using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class SelectWorkspaceWindow : WindowBase
	{
		private const string WorkspaceListAutomationId = "ItemListView";

		public SelectWorkspaceWindow(WindowDetails window)
			: base(window)
		{
		}

		public void ChooseWorkspace(string workspaceName)
		{
			EnterSearchText(workspaceName);
			Wait.For(() => GetWorkspaceCount() == 1);
			ClickWorkspace(0);
			ClickOkButton();
		}

		public void EnterSearchText(string searchText)
		{
			var searchTextBox = GetSearchTextBox();
			searchTextBox.SendKeys(searchText);
		}

		public void ClickOkButton()
		{
			Element.ClickButtonWithAutomationId("OKButton");
		}

		private void ClickWorkspace(int index)
		{
			var list = GetWorkspacesList();
			var listItemText = list.FindListItemTextByIndex(index);
			listItemText.Click();
		}

		private int GetWorkspaceCount()
		{
			var list = Element.WaitForChild(x => x.FindListsWithAutomationId(WorkspaceListAutomationId));
			return list.FindListItems().Count;
		}

		private AppiumWebElement GetWorkspacesList()
		{
			return Element.FindListWithAutomationId(WorkspaceListAutomationId);
		}

		private AppiumWebElement GetSearchTextBox()
		{
			return Element.FindEditWithAutomationId("SearchQuery");
		}
	}
}