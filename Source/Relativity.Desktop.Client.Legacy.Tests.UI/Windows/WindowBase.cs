using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	internal abstract class WindowBase : UIElement
	{
		protected WindowBase(WindowDetails window) : base(() => window.Element)
		{
			Handle = window.Handle;
		}

		public string Handle { get; }
	}
}