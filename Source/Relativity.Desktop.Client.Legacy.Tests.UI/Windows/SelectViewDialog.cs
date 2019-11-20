using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class SelectViewDialog : UIElement<SelectViewDialog>
	{
		private readonly ButtonUIElement selectButton;
		private readonly ListUIElement viewsList;

		public SelectViewDialog(Func<AppiumWebElement> create) : base(create)
		{
			viewsList = FindListWithAutomationId("selectionListBox");
			selectButton = FindButtonWithAutomationId("_selectButton");
		}

		public void SelectView(string name)
		{
			viewsList.SelectListItem(name);
			selectButton.Click();
		}
	}
}