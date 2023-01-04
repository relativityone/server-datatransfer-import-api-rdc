using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal sealed class UntrustedCertificateWindow : RdcWindowBase<UntrustedCertificateWindow>
	{
		private readonly ButtonUIElement allowButton;

		public UntrustedCertificateWindow(ILog logger, RdcWindowsManager windowsManager, WindowDetails window)
			: base(logger, windowsManager, window)
		{
			allowButton = FindButtonWithAutomationId("AllowButton");
		}

		public void ClickAllowButton()
		{
			allowButton.Click();
		}
	}
}