using System;
using System.IO;
using NUnit.Framework;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[TestFixture]
	internal class ExportTests : RdcTestBase
	{
		private const string ExportPath = @"C:\Adrian\Temp\Export\3";

		[Test]
		public void ExportToDatFile()
		{
			EnsureExportPathExists(ExportPath);

			AllowUntrustedCertificate();

			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace("Workspace-For-Export-Tests");
			rdcWindow.SelectRootFolder();

			var exportParameters = new ExportParameters
			{
				View = "Documents - All Metadata",
				ExportPath = ExportPath,
				VolumeInformationDigitPadding = 3,
				FilesNamedAfter = "Identifier",
				ExportImages = false,
				ExportNativeFiles = true,
				ExportFullTextAsFile = true,
				NativeFileFormat = "Concordance (.dat)",
				DataFileEncoding = "Unicode (UTF-8)",
				TextFileEncoding = "Unicode (UTF-8)",
				TextFieldPrecedence = "Extracted Text"
			};

			var exportWindow = rdcWindow.ExportFolderAndSubfolders();
			exportWindow.SetupExport(exportParameters);
			var progressWindow = exportWindow.RunExport();

			var allRecordsProcessed = progressWindow.WaitForAllRecordsToBeProcessed(TimeSpan.FromMinutes(2));
			Assert.IsTrue(allRecordsProcessed, "Failed to process all records");

			var errors = progressWindow.GetErrorsText();
			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Export failed with errors: {errors}");
		}

		private void EnsureExportPathExists(string exportPath)
		{
			var exportFolder = new DirectoryInfo(exportPath);
			if (exportFolder.Exists)
			{
				exportFolder.Delete(true);
				exportFolder.Create();
			}
			else
			{
				exportFolder.Create();
			}
		}
	}
}