using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;
using Relativity.DataExchange.TestFramework;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	internal abstract class RdcTestBase
	{
		private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
		private const string UserLogin = "relativity.admin@kcura.com";
		private const string Password = "Test1234!";

		private readonly string appId = GetRdcExePath();

		private WinAppDriverRunner driverRunner;
		protected RdcWindowsManager RdcWindowsManager;
		private WindowsDriver<WindowsElement> session;
		protected IntegrationTestParameters TestParameters;

		[SetUp]
		public void SetUp()
		{
			OnSetUp();

			var sessionFactory = new WindowsDriverSessionFactory(new Uri(WindowsApplicationDriverUrl));
			session = sessionFactory.CreateExeAppSession(appId);
			RdcWindowsManager = new RdcWindowsManager(new WindowsManager(session));
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

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			driverRunner = new WinAppDriverRunner();
			driverRunner.Run();
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			driverRunner.Dispose();
		}

		protected virtual void OnSetUp()
		{
		}

		protected virtual void OnTearDown()
		{
		}

		protected void AllowUntrustedCertificate()
		{
			//TODO: do this conditionally if window exists
			var untrustedCertificateWindow = RdcWindowsManager.SwitchToUntrustedCertificateWindow();
			untrustedCertificateWindow.ClickAllowButton();
		}

		protected SelectWorkspaceWindow Login()
		{
			var loginWindow = RdcWindowsManager.SwitchToLoginWindow();
			return loginWindow.Login(UserLogin, Password);
		}

		private static string GetRdcExePath()
		{
			return Path.Combine(GetRootPath(),
				@"Source\Relativity.Desktop.Client.Legacy\bin\Relativity.Desktop.Client.exe");
		}

		protected static string GetTestFilePath(string relativePath)
		{
			return Path.Combine(GetRootPath(), "TestFiles", relativePath);
		}

		private static string GetRootPath()
		{
			return new FileInfo(Assembly.GetExecutingAssembly().Location)
				.Directory.Parent.Parent.Parent.Parent.FullName;
		}
	}
}