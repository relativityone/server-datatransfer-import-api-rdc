namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	using System;
	using System.IO;

	using OpenQA.Selenium.Appium.Windows;
	using OpenQA.Selenium.Remote;

	public class WindowsDriverSessionFactory
	{
		private readonly Uri winAppDriverAddress;

		public WindowsDriverSessionFactory(Uri winAppDriverAddress)
		{
			this.winAppDriverAddress = winAppDriverAddress;
		}

		public WindowsDriver<WindowsElement> CreateExeAppSession(string appExePath)
		{
			FileInfo file = new FileInfo(appExePath);
			DesiredCapabilities appCapabilities = new DesiredCapabilities();
			appCapabilities.SetCapability("app", file.FullName);
			appCapabilities.SetCapability("appWorkingDir", file.Directory.FullName);
			return new WindowsDriver<WindowsElement>(this.winAppDriverAddress, appCapabilities);
		}

		public WindowsDriver<WindowsElement> CreateDesktopSession()
		{
			DesiredCapabilities appCapabilities = new DesiredCapabilities();
			appCapabilities.SetCapability("app", "Root");
			return new WindowsDriver<WindowsElement>(this.winAppDriverAddress, appCapabilities);
		}
	}
}