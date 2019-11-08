using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class DialogWindow : RdcWindowBase<DialogWindow>
	{
		public DialogWindow(RdcWindowsManager windowsManager, WindowDetails window) : base(windowsManager, window)
		{
		}

		public void ClickButton(string name)
		{
			var button = FindButton(name);
			button.Click();
		}
	}
}