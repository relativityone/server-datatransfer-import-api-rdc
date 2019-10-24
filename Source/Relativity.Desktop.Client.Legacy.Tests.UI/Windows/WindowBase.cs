namespace Relativity.Desktop.Client.Legacy.Tests.UI.Windows
{
	using OpenQA.Selenium.Appium.Windows;

	using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;

	internal abstract class WindowBase
	{
		protected WindowBase(WindowDetails window)
		{
			Window = window;
		}

		public WindowDetails Window { get; }

		protected WindowsElement Element => Window.Element;
	}
}