using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class SelectViewDialog : UIElement<SelectViewDialog>
	{
		private readonly ButtonUIElement selectButton;
		private readonly ListUIElement viewsList;
		private readonly EditUIElement searchTextBox;

		public SelectViewDialog(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			viewsList = FindListWithAutomationId("selectionListBox");
			selectButton = FindButtonWithAutomationId("_selectButton");
			searchTextBox = FindEditWithAutomationId("selectionSearchInput");
		}

		public void SelectView(string name)
		{
			searchTextBox.SetText(name);
			viewsList.WaitToHaveSingleElement();
			viewsList.SelectListItem(name);
			selectButton.Click();
		}
	}
}