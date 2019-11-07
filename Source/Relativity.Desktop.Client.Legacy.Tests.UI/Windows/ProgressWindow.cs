using System;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class ProgressWindow : RdcWindowBase
	{
		private readonly UIElement currentRecordLabel;

		private readonly TabsUIElement tabs;

		public ProgressWindow(RdcWindowsManager windowsManager, WindowDetails window) : base(
			windowsManager, window)
		{
			currentRecordLabel = FindTextWithAutomationId("_currentRecordLabel");
			tabs = new TabsUIElement(FindTabWithAutomationId("_Tabs"));
		}

		public bool WaitForAllRecordsToBeProcessed(TimeSpan timeout)
		{
			Wait.For(AreAllRecordsProcessed, TimeSpan.FromSeconds(2), timeout);
			return AreAllRecordsProcessed();
		}

		private bool AreAllRecordsProcessed()
		{
			return currentRecordLabel.Text == "All records have been processed";
		}

		public string GetErrorsText()
		{
			tabs.FindTabItem("Errors").Click();
			return tabs.FindEditWithAutomationId("TextBox").Text;
		}
	}
}