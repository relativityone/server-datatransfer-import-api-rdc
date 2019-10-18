// -----------------------------------------------------------------------------------------------------
// <copyright file="GoldPathExporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents all folder and sub-folder export integration tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Linq;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Feature.DataTransfer.RelativityDesktopClient.Export]
	[TestFixture]
	public class GoldPathExporterTests : ExporterTestBase
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static (ExportFile.ExportedFilePathType typeOfExportedFilePath, string filePrefix)?[] exportNatives =
			{
				null,
				(ExportFile.ExportedFilePathType.Absolute, null),
				(ExportFile.ExportedFilePathType.Prefix, @"C:\Prefix"),
				(ExportFile.ExportedFilePathType.Relative, null),
			};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static (LoadFileType.FileFormat logFileFormat, ExportFile.ImageType typeOfImage)?[] exportImages =
			{
				null,
				(LoadFileType.FileFormat.IPRO, ExportFile.ImageType.MultiPageTiff),
				(LoadFileType.FileFormat.IPRO_FullText, ExportFile.ImageType.SinglePage),
				(LoadFileType.FileFormat.IPRO_FullText, ExportFile.ImageType.Pdf),
				(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.SinglePage),
				(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.MultiPageTiff),
				(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.Pdf),
			};

		[IdentifiedTest("5b20c6f1-1196-41ea-9326-0e875e2cabe9")]
		[Test]
		[Pairwise]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public void ShouldExportAllSampleDocAndImages(
			[Values(TapiClient.Direct)] TapiClient client,
			[Values("utf-8", "utf-16")] string textFileEncoding,
			[Values("utf-8", "utf-16")] string loadFileEncoding,
			[ValueSource(nameof(exportNatives))](ExportFile.ExportedFilePathType typeOfExportedFilePath, string filePrefix)? exportNative,
			[ValueSource(nameof(exportImages))](LoadFileType.FileFormat logFileFormat, ExportFile.ImageType typeOfImage)? exportImage)
		{
			if ((client == TapiClient.Aspera && AssemblySetup.TestParameters.SkipAsperaModeTests) ||
				(client == TapiClient.Direct && AssemblySetup.TestParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}

			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);
			this.ExtendedExportFile.TextFileEncoding = Encoding.GetEncoding(textFileEncoding);
			this.ExtendedExportFile.LoadFileEncoding = Encoding.GetEncoding(loadFileEncoding);

			this.ExtendedExportFile.ExportNative = exportNative.HasValue;
			if (exportNative.HasValue)
			{
				this.ExtendedExportFile.TypeOfExportedFilePath = exportNative.Value.typeOfExportedFilePath;
				if (exportNative.Value.filePrefix != null)
				{
					this.ExtendedExportFile.FilePrefix = exportNative.Value.filePrefix;
				}
			}

			this.ExtendedExportFile.ExportImages = exportImage.HasValue;
			if (exportImage.HasValue)
			{
				this.ExtendedExportFile.LogFileFormat = exportImage.Value.logFileFormat;
				this.ExtendedExportFile.TypeOfImage = exportImage.Value.typeOfImage;
			}

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(ExporterTestData.AllSampleFiles.Count());
		}
	}
}