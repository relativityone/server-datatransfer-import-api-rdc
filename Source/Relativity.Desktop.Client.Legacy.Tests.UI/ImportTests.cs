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
		[Test]
		public void ImportDatLoadFileUsingSavedKweSettings()
		{
			RunImportTest(x =>
			{
				var importWindow = x.ImportDocumentLoadFile();

				var parameters = new RdoImportWindowSetupParameters
				{
					ImportFilePath = PathsProvider.GetTestInputFilePath(@"Production\Documents.dat"),
					SettingsFilePath = PathsProvider.GetTestInputFilePath(@"Production\Documents.kwe")
				};

				importWindow.SetupImport(parameters);
				importWindow.ClickImportFileMenuItem();
				RdcWindowsManager.GetRdcConfirmationDialog().ClickButton("OK");
				return RdcWindowsManager.SwitchToProgressWindow(ProgressWindowName.ImportLoadFileProgress);
			});
		}

		[Test]
		public void ImportProduction()
		{
			var parameters = new ImageImportWindowSetupParameters
			{
				ImportFilePath =
					PathsProvider.GetTestInputFilePath(@"Production\Production.opt"),
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
			var parameters = new ImageImportWindowSetupParameters
			{
				ImportFilePath = PathsProvider.GetTestInputFilePath(@"Images\Images.opt"),
				SettingsFilePath = PathsProvider.GetTestInputFilePath(@"Images\Images.kwi")
			};

			RunImportTest(x =>
			{
				var importWindow = x.ImportImageLoadFile();
				importWindow.SetupImport(parameters);
				return importWindow.RunImport();
			});
		}

		[Test]
		public void ImportImageProfiles()
		{
			RunImportTest(x =>
			{
				var importWindow = x.ImportImagingProfileObjects();

				var parameters = new RdoImportWindowSetupParameters
				{
					ImportFilePath = PathsProvider.GetTestInputFilePath(@"RDO\ImagingProfiles.dat"),
					AutoMapFields = true
				};

				importWindow.SetupImport(parameters);
				var progressWindow = importWindow.RunImport();
				return progressWindow;
			});
		}

		private void RunImportTest(Func<RelativityDesktopClientWindow, ProgressWindow> runImport)
		{
			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace(TestParameters.WorkspaceName);
			rdcWindow.SelectRootFolder();

			var progressWindow = runImport(rdcWindow);
			var allRecordsProcessed = progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(5));
			var progressStatus = progressWindow.StatusText;
			var errors = progressWindow.GetErrorsText();

			// Assert
			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Import failed with errors: {errors}");
			Assert.IsTrue(allRecordsProcessed, $"Failed to process all records. Status: {progressStatus}");
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