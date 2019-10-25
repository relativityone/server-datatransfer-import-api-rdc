using System;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class ImportLoadFileProgressWindow : RdcWindowBase
	{
		private readonly UIElement currentRecordLabel;

		private readonly TabsUIElement tabs;

		public ImportLoadFileProgressWindow(RdcWindowsManager windowsManager, WindowDetails window) : base(
			windowsManager, window)
		{
			currentRecordLabel = FindTextWithAutomationId("_currentRecordLabel");
			tabs = new TabsUIElement(FindTabWithAutomationId("_Tabs"));
		}

		public void WaitForAllRecordsToBeProcessed(TimeSpan timeout)
		{
			Wait.For(() => currentRecordLabel.Text == "All records have been processed", TimeSpan.FromSeconds(2),
				timeout);
		}

		public string GetErrorsText()
		{
			tabs.FindTabItem("Errors").Click();
			return tabs.FindEditWithAutomationId("TextBox").Text;
		}
	}
}