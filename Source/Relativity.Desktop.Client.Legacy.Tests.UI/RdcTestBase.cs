using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.Appium.Windows;
using Relativity.DataExchange.TestFramework;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	internal abstract class RdcTestBase
	{
		protected static readonly TestPathsProvider PathsProvider = new TestPathsProvider();
		private static readonly string Password = IntegrationTestHelper.IntegrationTestParameters.RelativityPassword;
		private static readonly string UserLogin = IntegrationTestHelper.IntegrationTestParameters.RelativityUserName;

		private static readonly string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
		protected RdcWindowsManager RdcWindowsManager;
		private WindowsDriver<WindowsElement> session;
		protected IntegrationTestParameters TestParameters;
		private string screenshotsFolder;

		[SetUp]
		public void SetUp()
		{
			OnSetUp();

			var sessionFactory = new WindowsDriverSessionFactory(new Uri(WindowsApplicationDriverUrl));
			session = sessionFactory.CreateExeAppSession(TestPathsProvider.RdcPath(IntegrationTestHelper.IntegrationTestParameters));
			RdcWindowsManager = new RdcWindowsManager(Relativity.Logging.Log.Logger, new WindowsManager(session));
			AllowUntrustedCertificate();

			screenshotsFolder = Path.Combine(
				TestPathsProvider.GetTestReportsDirectory(), "ui-automation-tests");
			CaptureScreenshot.SetUpScreenshotTool(screenshotsFolder);
		}

		[TearDown]
		public void TearDown()
		{
			CloseSession();
			OnTearDown();

			var testStatus = TestContext.CurrentContext.Result.Outcome.Status;

			if (testStatus != TestStatus.Skipped && testStatus != TestStatus.Passed)
			{
				CaptureScreenshot.GetInstance().CaptureDesktopScreenshot(Relativity.Logging.Log.Logger);
				throw new Exception($"================> SEE IMAGES FROM TEST EXECUTION: '{screenshotsFolder}' <================"); // The only way to add information to NUnit html log
			}
		}

		protected void CloseSession()
		{
			if (session != null)
			{
				RdcWindowsManager.SwitchToRelativityDesktopClientWindow();
				session.Close();
				session.Quit();
				session = null;
			}
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