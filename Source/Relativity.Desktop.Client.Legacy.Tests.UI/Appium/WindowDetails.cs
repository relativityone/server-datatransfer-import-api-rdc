namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	using OpenQA.Selenium.Appium.Windows;

	internal class WindowDetails
	{
		public WindowDetails(string windowHandle)
		{
			Handle = windowHandle;
		}

		public WindowDetails(string windowHandle, string title, WindowsElement windowsElement)
			: this(windowHandle)
		{
			Title = title;
			Element = windowsElement;
		}

		public string Handle { get; }

		public WindowsElement Element { get; }

		public string Title { get; }
	}
}