using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;
using Relativity.DataExchange.TestFramework;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[TestFixture]
	public class ImportTests
	{
		private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
		private const string Login = "relativity.admin@kcura.com";
		private const string Password = "Test1234!";

		private readonly string appId = GetRdcExePath();
		private readonly string datFile = GetTestFilePath(@"1\Documents_export.dat");
		private readonly string kweFile = GetTestFilePath(@"1\Documents_export.kwe");
		private WinAppDriverRunner driverRunner;
		private RdcWindowsManager rdcWindowsManager;
		private WindowsDriver<WindowsElement> session;
		private IntegrationTestParameters testParameters;

		[SetUp]
		public void SetUp()
		{
			CreateWorkspace();

			var sessionFactory = new WindowsDriverSessionFactory(new Uri(WindowsApplicationDriverUrl));
			session = sessionFactory.CreateExeAppSession(appId);
			rdcWindowsManager = new RdcWindowsManager(new WindowsManager(session));
		}

		[TearDown]
		public void TearDown()
		{
			rdcWindowsManager.SwitchToRelativityDesktopClientWindow();
			session.Close();
			session.Quit();
			session = null;

			RemoveWorkspace();
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

		[Explicit]
		[Test]
		public void LoadFileImport()
		{
			var loginWindow = rdcWindowsManager.SwitchToLoginWindow();
			var workspaceSelectWindow = loginWindow.Login(Login, Password);

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace(testParameters.WorkspaceName);
			rdcWindow.SelectRootFolder();

			var importWindow = rdcWindow.ImportDocumentLoadFile();
			importWindow.LoadImportSettings();
			importWindow.LoadKweFile(kweFile, datFile);
			importWindow.ClickImportFileMenuItem();

			rdcWindowsManager.GetRdcConfirmationDialog().ClickButton("OK");

			var progressWindow = rdcWindowsManager.SwitchToImportLoadFileProgressWindow();
			progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(2));
			var errors = progressWindow.GetErrorsText();

			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Export failed with errors: {errors}");
		}

		private void CreateWorkspace()
		{
			testParameters = IntegrationTestHelper.Create();
		}

		private void RemoveWorkspace()
		{
			if (testParameters != null)
			{
				IntegrationTestHelper.Destroy(testParameters);
				testParameters = null;
			}
		}

		private static string GetRdcExePath()
		{
			return Path.Combine(GetRootPath(),
				@"Source\Relativity.Desktop.Client.Legacy\bin\Relativity.Desktop.Client.exe");
		}

		private static string GetTestFilePath(string relativePath)
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