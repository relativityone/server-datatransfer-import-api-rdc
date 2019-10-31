using System;
using System.IO;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class WindowsDriverSessionFactory
	{
		private readonly Uri winAppDriverAddress;

		public WindowsDriverSessionFactory(Uri winAppDriverAddress)
		{
			this.winAppDriverAddress = winAppDriverAddress;
		}

		public WindowsDriver<WindowsElement> CreateExeAppSession(string appExePath)
		{
			var file = new FileInfo(appExePath);
			var appCapabilities = new DesiredCapabilities();
			appCapabilities.SetCapability("app", file.FullName);
			appCapabilities.SetCapability("appWorkingDir", file.Directory.FullName);
			return CreateSession(appCapabilities);
		}

		public WindowsDriver<WindowsElement> CreateDesktopSession()
		{
			var appCapabilities = new DesiredCapabilities();
			appCapabilities.SetCapability("app", "Root");
			return CreateSession(appCapabilities);
		}

		private WindowsDriver<WindowsElement> CreateSession(DesiredCapabilities appCapabilities)
		{
			var session = new WindowsDriver<WindowsElement>(winAppDriverAddress, appCapabilities);
			session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1.5);
			return session;
		}
	}
}