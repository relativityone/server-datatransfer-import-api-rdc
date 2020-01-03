﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="PositiveExportTests.cs" company="Relativity ODA LLC">
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

	using Relativity.DataExchange.Export.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Feature.DataTransfer.RelativityDesktopClient.Export]
	[Category(TestCategories.Export)]
	[Category(TestCategories.Integration)]
	public class PositiveExportTests : ExportTestBase
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly ExportTypeDto[] ExportTypes =
		{
			new ExportTypeDto(ExportFile.ExportType.AncestorSearch),
			new ExportTypeDto(ExportFile.ExportType.ParentSearch),
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly ExportNativeDto[] ExportNatives =
		{
			null,
			new ExportNativeDto(ExportFile.ExportedFilePathType.Absolute, null),
			new ExportNativeDto(ExportFile.ExportedFilePathType.Prefix, @"C:\filePrefix"),
			new ExportNativeDto(ExportFile.ExportedFilePathType.Relative, null),
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly ExportImageDto[] ExportImages =
		{
			null,
			new ExportImageDto(LoadFileType.FileFormat.IPRO, ExportFile.ImageType.MultiPageTiff),
			new ExportImageDto(LoadFileType.FileFormat.IPRO_FullText, ExportFile.ImageType.SinglePage),
			new ExportImageDto(LoadFileType.FileFormat.IPRO_FullText, ExportFile.ImageType.Pdf),
			new ExportImageDto(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.SinglePage),
			new ExportImageDto(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.MultiPageTiff),
			new ExportImageDto(LoadFileType.FileFormat.Opticon, ExportFile.ImageType.Pdf),
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly PaddingDto[] Paddings =
		{
			new PaddingDto(3, 2),
			new PaddingDto(5, 6),
		};

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly DelimiterDto[] Delimiters =
		{
			new DelimiterDto(';', '\\', '@', 'þ', '¶'),
			new DelimiterDto(':', '/', '\n', '"', '\r'),
		};

		[IdentifiedTest("5b20c6f1-1196-41ea-9326-0e875e2cabe9")]
		[Test]
		[Pairwise]
		public void ShouldExportAllSampleDocAndImages(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[ValueSource(nameof(ExportTypes))] ExportTypeDto exportType,
			[Values("utf-8", "utf-16")] string textFileEncoding,
			[Values("utf-8", "utf-16")] string loadFileEncoding,
			[ValueSource(nameof(ExportNatives))] ExportNativeDto exportNative,
			[ValueSource(nameof(ExportImages))] ExportImageDto exportImage,
			[ValueSource(nameof(Paddings))] PaddingDto paddingValue,
			[ValueSource(nameof(Delimiters))] DelimiterDto delimiterValue)
		{
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			// ARRANGE
			GivenTheTapiForceClientAppSettings(client);

			// TODO REL-369935 enable ExportFile.ExportType.ArtifactSearch and ExportFile.ExportType.Production
			this.ExtendedExportFile.TypeOfExport = exportType.ExportType;

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
			// TODO REL-369935 add much better validation for each of the results.
			this.ThenTheExportJobIsSuccessful(TestData.SampleFiles.Count());
		}
	}
}