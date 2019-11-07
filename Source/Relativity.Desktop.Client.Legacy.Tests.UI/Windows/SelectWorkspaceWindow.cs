using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class SelectWorkspaceWindow : RdcWindowBase
	{
		private const string WorkspaceListAutomationId = "ItemListView";

		private readonly UIElement okButton;
		private readonly UIElement searchTextBox;
		private readonly ListUIElement workspaceList;

		public SelectWorkspaceWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			okButton = FindButtonWithAutomationId("OKButton");
			searchTextBox = FindEditWithAutomationId("SearchQuery");
			workspaceList = new ListUIElement(WaitForListWithAutomationId(WorkspaceListAutomationId));
		}

		public RelativityDesktopClientWindow ChooseWorkspace(string workspaceName)
		{
			EnterSearchText(workspaceName);
			Wait.For(() => workspaceList.ItemsCount == 1);
			SelectWorkspace(workspaceName);
			ClickOkButton();
			return WindowsManager.SwitchToRelativityDesktopClientWindow();
		}

		public void EnterSearchText(string searchText)
		{
			searchTextBox.SendKeys(searchText);
		}

		public void ClickOkButton()
		{
			okButton.Click();
		}

		private void SelectWorkspace(string workspaceName)
		{
			workspaceList.SelectListItem(workspaceName);
		}
	}
}