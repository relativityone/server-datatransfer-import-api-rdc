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
	[Category(TestCategories.Export)]
	[Category(TestCategories.Integration)]
	public class GoldPathExporterTests : ExporterTestBase
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly ExportNativeValue[] ExportNatives =
		{
			null,
			new ExportNativeValue(ExportFile.ExportedFilePathType.Absolute, null),
			new ExportNativeValue(ExportFile.ExportedFilePathType.Prefix, @"C:\filePrefix"),
			new ExportNativeValue(ExportFile.ExportedFilePathType.Relative, null),
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly ExportImageValue[] ExportImages =
		{
			null,
			new ExportImageValue(LoadFileType.FileFormat.IPRO, ExportFile.ImageType.MultiPageTiff),
			new ExportImageValue(LoadFileType.FileFormat.IPRO_FullText, ExportFile.ImageType.SinglePage),
			new ExportImageValue(LoadFileType.FileFormat.IPRO_FullText, ExportFile.ImageType.Pdf),
			new ExportImageValue(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.SinglePage),
			new ExportImageValue(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.MultiPageTiff),
			new ExportImageValue(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.Pdf),
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly PaddingValue[] Paddings =
		{
			new PaddingValue(3, 2),
			new PaddingValue(5, 6),
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly DelimiterValue[] Delimiters =
		{
			new DelimiterValue(';', '\\', '@', 'þ', '¶'),
			new DelimiterValue(':', '/', '\n', '"', '\r'),
		};

		[IdentifiedTest("5b20c6f1-1196-41ea-9326-0e875e2cabe9")]
		[Test]
		[Pairwise]
		public void ShouldExportAllSampleDocAndImages(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(ExportFile.ExportType.AncestorSearch, ExportFile.ExportType.ParentSearch)] ExportFile.ExportType exportType,
			[Values("utf-8", "utf-16")] string textFileEncoding,
			[Values("utf-8", "utf-16")] string loadFileEncoding,
			[ValueSource(nameof(ExportNatives))] ExportNativeValue exportNative,
			[ValueSource(nameof(ExportImages))] ExportImageValue exportImage,
			[ValueSource(nameof(Paddings))] PaddingValue paddingValue,
			[ValueSource(nameof(Delimiters))] DelimiterValue delimiterValue)
		{
			if ((client == TapiClient.Aspera && AssemblySetup.TestParameters.SkipAsperaModeTests) ||
				(client == TapiClient.Direct && AssemblySetup.TestParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}

			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);

			// TODO enable ExportFile.ExportType.ArtifactSearch and ExportFile.ExportType.Production
			this.ExtendedExportFile.TypeOfExport = exportType;

			this.ExtendedExportFile.TextFileEncoding = Encoding.GetEncoding(textFileEncoding);
			this.ExtendedExportFile.LoadFileEncoding = Encoding.GetEncoding(loadFileEncoding);

			this.ExtendedExportFile.ExportNative = exportNative != null;
			if (this.ExtendedExportFile.ExportNative)
			{
				this.ExtendedExportFile.TypeOfExportedFilePath = exportNative.ExportedFilePathType;
				if (exportNative.FilePrefix != null)
				{
					this.ExtendedExportFile.FilePrefix = exportNative.FilePrefix;
				}
			}

			this.ExtendedExportFile.ExportImages = exportImage != null;
			if (this.ExtendedExportFile.ExportImages)
			{
				this.ExtendedExportFile.LogFileFormat = exportImage.FileFormat;
				this.ExtendedExportFile.TypeOfImage = exportImage.ImageType;
			}

			this.ExtendedExportFile.SubdirectoryDigitPadding = paddingValue.SubdirectoryDigitPadding;
			this.ExtendedExportFile.VolumeDigitPadding = paddingValue.VolumeDigitPadding;

			this.ExtendedExportFile.MultiRecordDelimiter = delimiterValue.MultiRecordDelimiter;
			this.ExtendedExportFile.NestedValueDelimiter = delimiterValue.NestedValueDelimiter;
			this.ExtendedExportFile.NewlineDelimiter = delimiterValue.NewlineDelimiter;
			this.ExtendedExportFile.QuoteDelimiter = delimiterValue.QuoteDelimiter;
			this.ExtendedExportFile.RecordDelimiter = delimiterValue.RecordDelimiter;

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			// TODO add much better validation for each of the results.
			this.ThenTheExportJobIsSuccessful(ExporterTestData.AllSampleFiles.Count());
		}
	}
}