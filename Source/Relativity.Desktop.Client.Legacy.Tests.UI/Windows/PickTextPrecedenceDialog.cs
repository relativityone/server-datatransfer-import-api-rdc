using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class PickTextPrecedenceDialog : UIElement<PickTextPrecedenceDialog>
	{
		private readonly ListUIElement leftList;
		private readonly ButtonUIElement moveFieldRightButton;
		private readonly ButtonUIElement okButton;
		private readonly EditUIElement leftSearchTextBox;

		public PickTextPrecedenceDialog(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
			okButton = FindButtonWithAutomationId("_okButton");
			moveFieldRightButton = FindButtonWithAutomationId("_moveFieldRight");
			leftList = FindPaneWithAutomationId("_searchableListLeft").FindList();
			leftSearchTextBox = FindPaneWithAutomationId("_searchableListLeft").FindEditWithAutomationId("_textBox");
		}

		public void SelectFieldAndClose(string name)
		{
			leftSearchTextBox.SetText(name);
			leftList.WaitToHaveSingleElement();
			leftList.SelectListItem(name);
			moveFieldRightButton.Click();
			okButton.Click();
		}
	}
}