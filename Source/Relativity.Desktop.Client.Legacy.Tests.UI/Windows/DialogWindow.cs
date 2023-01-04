using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class DialogWindow : RdcWindowBase<DialogWindow>
	{
		public DialogWindow(ILog logger, RdcWindowsManager windowsManager, WindowDetails window) : base(logger,
			windowsManager, window)
		{
		}

		public void ClickButton(string name)
		{
			var button = FindButton(name);
			button.Click();
		}
	}
}