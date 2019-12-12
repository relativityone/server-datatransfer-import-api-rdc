using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal abstract class RdcWindowBase<T> : WindowBase<T> where T : UIElement<T>
	{
		protected readonly RdcWindowsManager WindowsManager;

		protected RdcWindowBase(ILog logger, RdcWindowsManager windowsManager, WindowDetails window)
			: base(logger, window)
		{
			WindowsManager = windowsManager;
		}

		public void SwitchToWindow()
		{
			WindowsManager.SwitchToWindow(Handle);
		}
	}
}