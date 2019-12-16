using System;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class SelectWorkspaceWindow : RdcWindowBase<SelectWorkspaceWindow>
	{
		private const string WorkspaceListAutomationId = "ItemListView";

		private readonly ButtonUIElement okButton;
		private readonly EditUIElement searchTextBox;
		private readonly ListUIElement workspaceList;

		public SelectWorkspaceWindow(ILog logger, RdcWindowsManager windowsManager, WindowDetails window)
			: base(logger, windowsManager, window)
		{
			okButton = FindButtonWithAutomationId("OKButton");
			searchTextBox = FindEditWithAutomationId("SearchQuery");
			workspaceList = FindListWithAutomationId(WorkspaceListAutomationId).WaitFor(TimeSpan.FromMilliseconds(500));
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