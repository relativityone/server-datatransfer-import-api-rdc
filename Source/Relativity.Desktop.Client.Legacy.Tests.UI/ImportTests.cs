namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	using System;
	using System.Threading;

	using NUnit.Framework;

	using Relativity.Desktop.Client.Legacy.Tests.UI.Appium;
	using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;

	[TestFixture]
	public class ImportTests
	{
		private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";

		private const string AppId =
			@"C:\Adrian\Repo\DataTransfer\Import-API-RDC\Source\Relativity.Desktop.Client.Legacy\bin\Relativity.Desktop.Client.exe";

		private const string KweFile =
			@"C:\Users\adrian.stanula\Downloads\Post-installation_verification_test_data\Post-installation verification test data\Salt-v-Pepper-kCura-Starter-Template.kwe";

		private const string DatFile =
			@"C:\Users\adrian.stanula\Downloads\Post-installation_verification_test_data\Post-installation verification test data\Salt-v-Pepper (US date format).dat";

		[Explicit]
		[Test]
		public static void LoadFileImport()
		{
			var appDriverUrl = new Uri(WindowsApplicationDriverUrl);
			var sessionFactory = new WindowsDriverSessionFactory(appDriverUrl);
			var session = sessionFactory.CreateExeAppSession(AppId);
			var manager = new WindowsManager(session);

			var loginWindowDetails = manager.GetWindow(WindowNames.RelativityLogin);
			var loginWindow = new LoginWindow(loginWindowDetails.WindowsElement);

			loginWindow.Login("relativity.admin@kcura.com", "Test1234!");

			//TODO: Implement a better wait to wait for a window to open
			Thread.Sleep(7000);

			var selectWorkspaceWindowDetails = manager.GetWindow(WindowNames.SelectWorkspace);
			var workspaceSelectWindow = new SelectWorkspaceWindow(selectWorkspaceWindowDetails.WindowsElement);

			workspaceSelectWindow.EnterSearchText("Test1");
			//TODO: Implement a better method to wait for some event to occur
			Wait.Small();
			workspaceSelectWindow.ClickWorkspace(0);
			Wait.Tiny();
			workspaceSelectWindow.ClickOkButton();
			Wait.Small();

			var rdcWindowDetails = manager.GetWindow(WindowNames.RelativityDesktopClient);
			var rdcWindow = new RelativityDesktopClientWindow(rdcWindowDetails.WindowsElement);
			var workspaceRootFolder = rdcWindow.GetRootFolder();
			workspaceRootFolder.Click();

			rdcWindow.ClickDocumentLoadFileMenuItem();
			Wait.Second();

			var importWindowsElement = manager.GetWindow(WindowNames.ImportDocumentLoadFile);
			var importWindow = new ImportDocumentLoadFileWindow(importWindowsElement.WindowsElement);
			importWindow.ClickLoadImportSettingsMenuItem();
			Wait.Second();

			importWindow.LoadKweFile(KweFile, DatFile);
			importWindow.ClickImportFileMenuItem();
			Wait.HalfASecond();

			var confirmationDialog = manager.GetWindow(
				WindowNames.RelativityDesktopClient,
				x => x.WindowHandle != rdcWindowDetails.WindowHandle).WindowsElement;

			var okButton = confirmationDialog.FindButton("OK");
			okButton.Click();

			session.Close();
			session.Quit();
		}
	}
}