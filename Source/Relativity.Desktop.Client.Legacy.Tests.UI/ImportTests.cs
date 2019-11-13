using System;
using NUnit.Framework;
using Relativity.DataExchange.TestFramework;
using Relativity.DataExchange.TestFramework.RelativityHelpers;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[TestFixture]
	internal class ImportTests : RdcTestBase
	{
		private readonly string documentsImportDatFile =
			PathsProvider.GetTestInputFilePath(@"ImportTests\Documents_export.dat");

		private readonly string documentsImportSettingsFile =
			PathsProvider.GetTestInputFilePath(@"ImportTests\Documents_export.kwe");

		private readonly string imagesImportFile = PathsProvider.GetTestInputFilePath(@"Images\Images.opt");
		private readonly string imagesImportSettingsFile = PathsProvider.GetTestInputFilePath(@"Images\Images.kwi");

		private readonly string productionImportFile =
			PathsProvider.GetTestInputFilePath(@"ProductionSet\Production-Set-To-Export_export.opt");

		[Test]
		public void ImportDatLoadFileUsingSavedKweSettings()
		{
			RunImportTest(x =>
			{
				var importWindow = x.ImportDocumentLoadFile();
				importWindow.LoadKweFile(documentsImportSettingsFile, documentsImportDatFile);
				importWindow.ClickImportFileMenuItem();
				RdcWindowsManager.GetRdcConfirmationDialog().ClickButton("OK");
				return RdcWindowsManager.SwitchToProgressWindow(ProgressWindowName.ImportLoadFileProgress);
			});
		}

		[Test]
		public void ImportProduction()
		{
			var parameters = new ImportWindowSetupParameters
			{
				ImportFilePath = productionImportFile,
				ProductionName = "Imported-Production-Set",
				OverwriteMode = "Append Only"
			};

			ProductionHelper.CreateProduction(TestParameters, parameters.ProductionName, "BATES",
				IntegrationTestHelper.Logger);

			RunImportTest(x =>
			{
				var importWindow = x.ImportProductionLoadFile();
				importWindow.SetupImport(parameters);
				return importWindow.RunImport();
			});
		}

		[Test]
		public void ImportImages()
		{
			var parameters = new ImportWindowSetupParameters
			{
				ImportFilePath = imagesImportFile,
				SettingsFilePath = imagesImportSettingsFile
			};

			RunImportTest(x =>
			{
				var importWindow = x.ImportImageLoadFile();
				importWindow.SetupImport(parameters);
				return importWindow.RunImport();
			});
		}

		private void RunImportTest(Func<RelativityDesktopClientWindow, ProgressWindow> runImport)
		{
			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace(TestParameters.WorkspaceName);
			rdcWindow.SelectRootFolder();

			var progressWindow = runImport(rdcWindow);
			var allRecordsProcessed = progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(2));
			var errors = progressWindow.GetErrorsText();

			// Assert
			Assert.IsTrue(allRecordsProcessed, "Failed to process all records");
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