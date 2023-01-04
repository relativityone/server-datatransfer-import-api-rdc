// <copyright file="ExtendedExportFileSetup.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Text;

	using kCura.WinEDDS;

	internal static class ExtendedExportFileSetup
	{
		public static void SetupDelimiters(ExtendedExportFile extendedExportFile)
		{
			extendedExportFile.MultiRecordDelimiter = ':';
			extendedExportFile.NestedValueDelimiter = '/';
			extendedExportFile.NewlineDelimiter = '\n';
			extendedExportFile.QuoteDelimiter = '"';
			extendedExportFile.RecordDelimiter = '\r';
		}

		public static void SetupPaddings(ExtendedExportFile extendedExportFile)
		{
			extendedExportFile.SubdirectoryDigitPadding = 3;
			extendedExportFile.VolumeDigitPadding = 2;
		}

		public static void SetupImageExport(ExtendedExportFile extendedExportFile)
		{
			extendedExportFile.ExportImages = true;
			extendedExportFile.LogFileFormat = LoadFileType.FileFormat.Opticon;
			extendedExportFile.TypeOfImage = ExportFile.ImageType.SinglePage;
		}

		public static void SetupDocumentExport(ExtendedExportFile extendedExportFile)
		{
			var encoding = Encoding.GetEncoding("utf-8");
			extendedExportFile.TextFileEncoding = encoding;
			extendedExportFile.LoadFileEncoding = encoding;
			extendedExportFile.TypeOfExport = ExportFile.ExportType.AncestorSearch;
			extendedExportFile.ExportNative = true;
			extendedExportFile.TypeOfExportedFilePath = ExportFile.ExportedFilePathType.Absolute;
		}
	}
}