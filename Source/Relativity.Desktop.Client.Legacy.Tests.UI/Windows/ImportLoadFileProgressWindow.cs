using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class ImportLoadFileProgressWindow : WindowBase
	{
		public ImportLoadFileProgressWindow(WindowDetails window) : base(window)
		{
		}

		public void WaitForAllRecordsToBeProcessed(TimeSpan timeout)
		{
			var currentRecordLabel = Element.FindTextWithAutomationId("_currentRecordLabel");
			Wait.For(() => currentRecordLabel.Text == "All records have been processed", TimeSpan.FromSeconds(2),
				timeout);
		}

		public string GetErrorsText()
		{
			var tabs = GetTabsElement();
			tabs.FindTabItem("Errors").Click();
			return tabs.FindEditWithAutomationId("TextBox").Text;
		}

		private AppiumWebElement GetTabsElement()
		{
			return Element.FindTabWithAutomationId("_Tabs");
		}
	}
}