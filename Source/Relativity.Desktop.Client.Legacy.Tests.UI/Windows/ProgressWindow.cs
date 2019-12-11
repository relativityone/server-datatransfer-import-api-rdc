using System;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class ProgressWindow : RdcWindowBase<ProgressWindow>
	{
		private readonly TextUIElement currentRecordLabel;

		private readonly TabsUIElement tabs;

		public string StatusText => currentRecordLabel.Text;

		public ProgressWindow(ILog logger, RdcWindowsManager windowsManager, WindowDetails window)
			: base(
				logger,
				windowsManager,
				window)
		{
			currentRecordLabel = FindTextWithAutomationId("_currentRecordLabel");
			tabs = FindTabsWithAutomationId("_Tabs");
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