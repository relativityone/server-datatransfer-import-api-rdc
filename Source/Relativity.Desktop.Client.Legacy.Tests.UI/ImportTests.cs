using System;
using NUnit.Framework;
using Relativity.DataExchange.TestFramework;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[TestFixture]
	internal class ImportTests : RdcTestBase
	{
		private readonly string datFile = GetTestFilePath(@"1\Documents_export.dat");
		private readonly string kweFile = GetTestFilePath(@"1\Documents_export.kwe");

		[Test]
		public void LoadFileImport()
		{
			AllowUntrustedCertificate();

			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace(TestParameters.WorkspaceName);
			rdcWindow.SelectRootFolder();

			var importWindow = rdcWindow.ImportDocumentLoadFile();
			importWindow.LoadKweFile(kweFile, datFile);
			importWindow.ClickImportFileMenuItem();

			RdcWindowsManager.GetRdcConfirmationDialog().ClickButton("OK");

			var progressWindow = RdcWindowsManager.SwitchToImportLoadFileProgressWindow();
			progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(2));
			var errors = progressWindow.GetErrorsText();

			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Import failed with errors: {errors}");
		}

		protected override void OnSetUp()
		{
			CreateWorkspace();
		}

		protected override void OnTearDown()
		{
			RemoveWorkspace();
		}

		private void CreateWorkspace()
		{
			TestParameters = IntegrationTestHelper.Create();
		}

		private void RemoveWorkspace()
		{
			if (TestParameters != null)
			{
				IntegrationTestHelper.Destroy(TestParameters);
				TestParameters = null;
			}
		}
	}
}