using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal abstract class RdcWindowBase<T> : WindowBase<T> where T : UIElement<T>
	{
		protected readonly RdcWindowsManager WindowsManager;

		protected RdcWindowBase(RdcWindowsManager windowsManager, WindowDetails window)
			: base(window)
		{
			WindowsManager = windowsManager;
		}
	}
}