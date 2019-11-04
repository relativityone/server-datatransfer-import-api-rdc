using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Relativity.Desktop.Client.Legacy.Tests.UI.Windows;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[TestFixture]
	internal class ExportTests : RdcTestBase
	{
		private readonly string exportPath = PathsProvider.GetTestOutputPath(@"Export");

		[Test]
		public void ExportNativesWithFullTextAsFilesFromFolderAndSubfoldersToDatFile()
		{
			EnsureExportPathExists();

			var workspaceSelectWindow = Login();

			var rdcWindow = workspaceSelectWindow.ChooseWorkspace("Workspace-For-Export-Tests");
			rdcWindow.SelectRootFolder();

			var exportParameters = new ExportParameters
			{
				View = "Documents - All Metadata",
				ExportPath = exportPath,
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
			var errors = progressWindow.GetErrorsText();
			var files = Directory.GetFiles(exportPath, "*.*", SearchOption.AllDirectories);
			var filesSize = files.Sum(x => x.Length);
			Directory.Delete(exportPath, true);

			// Assert
			Assert.IsTrue(allRecordsProcessed, "Failed to process all records");
			Assert.IsTrue(string.IsNullOrEmpty(errors), $"Export failed with errors: {errors}");
			Assert.AreEqual(1805, files.Length);
			Assert.AreEqual(186810, filesSize);
		}

		private void EnsureExportPathExists()
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