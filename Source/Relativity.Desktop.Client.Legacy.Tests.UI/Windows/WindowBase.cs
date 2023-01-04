using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal abstract class WindowBase<T> : UIElement<T>, IWindow where T : UIElement<T>
	{
		protected WindowBase(ILog logger, WindowDetails window) : base(logger, () => window.Element)
		{
			Handle = window.Handle;
		}

		public string Handle { get; }
	}
}