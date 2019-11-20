using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class UntrustedCertificateWindow : RdcWindowBase<UntrustedCertificateWindow>
	{
		private readonly ButtonUIElement allowButton;

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