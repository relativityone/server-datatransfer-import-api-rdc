using System;
using System.Configuration;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;
using Relativity.DataExchange.TestFramework;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	internal abstract class RdcTestBase
	{
		protected static readonly TestPathsProvider PathsProvider = new TestPathsProvider();
		private static readonly string Password = ConfigurationManager.AppSettings["RelativityPassword"];
		private static readonly string UserLogin = ConfigurationManager.AppSettings["RelativityUserName"];

		private static readonly string WindowsApplicationDriverUrl =
			ConfigurationManager.AppSettings["WindowsApplicationDriverUrl"];

		protected RdcWindowsManager RdcWindowsManager;
		private WindowsDriver<WindowsElement> session;
		protected IntegrationTestParameters TestParameters;

		[SetUp]
		public void SetUp()
		{
			OnSetUp();

			var sessionFactory = new WindowsDriverSessionFactory(new Uri(WindowsApplicationDriverUrl));
			session = sessionFactory.CreateExeAppSession(PathsProvider.RdcPath);
			RdcWindowsManager = new RdcWindowsManager(new WindowsManager(session));

			AllowUntrustedCertificate();
		}

		[TearDown]
		public void TearDown()
		{
			RdcWindowsManager.SwitchToRelativityDesktopClientWindow();
			session.Close();
			session.Quit();
			session = null;

			OnTearDown();
		}

		protected virtual void OnSetUp()
		{
		}

		protected virtual void OnTearDown()
		{
		}

		private void AllowUntrustedCertificate()
		{
			if (RdcWindowsManager.TryGetUntrustedCertificateWindow(out var untrustedCertificateWindow))
			{
				untrustedCertificateWindow.ClickAllowButton();
			}
		}

		protected SelectWorkspaceWindow Login()
		{
			var loginWindow = RdcWindowsManager.SwitchToLoginWindow();
			return loginWindow.Login(UserLogin, Password);
		}
	}
}