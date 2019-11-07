using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal class UntrustedCertificateWindow : RdcWindowBase
	{
		private readonly UIElement allowButton;

		public UntrustedCertificateWindow(RdcWindowsManager windowsManager, WindowDetails window)
			: base(windowsManager, window)
		{
			allowButton = FindButtonWithAutomationId("AllowButton");
		}

		public void ClickAllowButton()
		{
			allowButton.Click();
		}
	}
}