using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal abstract class WindowBase<T> : UIElement<T>, IWindow where T : UIElement<T>
	{
		protected WindowBase(WindowDetails window) : base(() => window.Element)
		{
			Handle = window.Handle;
		}

		public string Handle { get; }
	}
}