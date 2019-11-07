using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class SelectViewDialog : UIElement
	{
		private readonly UIElement selectButton;
		private readonly ListUIElement viewsList;

		public SelectViewDialog(Func<AppiumWebElement> create) : base(create)
		{
			viewsList = new ListUIElement(FindListWithAutomationId("selectionListBox"));
			selectButton = FindButtonWithAutomationId("_selectButton");
		}

		public void SelectView(string name)
		{
			viewsList.SelectListItem(name);
			selectButton.Click();
		}
	}
}