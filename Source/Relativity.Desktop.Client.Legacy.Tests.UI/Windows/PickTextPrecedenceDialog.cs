using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	public class PickTextPrecedenceDialog : UIElement
	{
		private readonly ListUIElement leftList;
		private readonly UIElement moveFieldRightButton;
		private readonly UIElement okButton;

		public PickTextPrecedenceDialog(Func<AppiumWebElement> create) : base(create)
		{
			okButton = FindButtonWithAutomationId("_okButton");
			moveFieldRightButton = FindButtonWithAutomationId("_moveFieldRight");
			leftList = new ListUIElement(FindPaneWithAutomationId("_searchableListLeft").FindList());
		}

		public void SelectFieldsAndClose(params string[] names)
		{
			foreach (var name in names) leftList.SelectListItem(name);
			moveFieldRightButton.Click();
			okButton.Click();
		}
	}
}