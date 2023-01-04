using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Relativity.DataExchange.TestFramework;
using Relativity.DataExchange.TestFramework.RelativityHelpers;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows.Names;
using Relativity.Desktop.Client.Legacy.Tests.UI.Workflow;
using Relativity.Testing.Identification;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[TestFixture]
	[Feature.DataTransfer.RelativityDesktopClient.Import]
	internal class ImportTests : RdcTestBase
	{
		[IdentifiedTest("446c56de-7c5a-439c-b8c4-05107d1749a1")]
		[Feature.DataTransfer.RelativityDesktopClient.Import.Documents]
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

		[IdentifiedTest("9579f3d7-63d4-4132-b8f8-13da1d8f7e5a")]
		[Feature.DataTransfer.RelativityDesktopClient.Import.Productions]
		public async Task ImportProductionAsync()
		{
			var parameters = new ImageImportWindowSetupParameters
			{
				ImportFilePath =
					PathsProvider.GetTestInputFilePath(@"Production\Production.opt"),
				ProductionName = "Imported-Production-Set",
				OverwriteMode = "Append Only"
			};

			await ProductionHelper.CreateProductionAsync(TestParameters, parameters.ProductionName, "BATES").ConfigureAwait(false);

			RunImportTest(x =>
			{
				var importWindow = x.ImportProductionLoadFile();
				importWindow.SetupImport(parameters);
				return importWindow.RunImport();
			});
		}

		[IdentifiedTest("a22b3f16-43d6-489f-9465-5701acfff699")]
		[Feature.DataTransfer.RelativityDesktopClient.Import.Images]
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

		[IdentifiedTest("6905874b-bd5d-45f9-b125-105282ca3c0a")]
		[Feature.DataTransfer.RelativityDesktopClient.Import.DynamicObjects]
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
			rdcWindow.WaitForTransferModeDetection();

			var progressWindow = runImport(rdcWindow);
			var allRecordsProcessed = progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(5));

			if (RdcWindowsManager.TryGetRdcConfirmationDialog(out DialogWindow confirmationDialog))
			{
				confirmationDialog.ClickButton("Cancel");
				progressWindow.SwitchToWindow();
			}

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