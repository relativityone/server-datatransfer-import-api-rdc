// ----------------------------------------------------------------------------
// <copyright file="PdfFileExportRequestBuilder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	using Castle.Core;

	public class PdfFileExportRequestBuilder : ExportRequestBuilder
	{
		public PdfFileExportRequestBuilder(PdfFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator, IFileProcessingStatistics fileProcessingStatistics, ILog logger)
			: base(filePathProvider, fileNameProvider, validator, fileProcessingStatistics, logger)
		{
		}

		/// <summary>
		/// Used for testing
		/// </summary>
		/// <param name="filePathProvider"></param>
		/// <param name="fileNameProvider"></param>
		/// <param name="validator"></param>
		/// <param name="fileProcessingStatistics"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public PdfFileExportRequestBuilder(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics, ILog logger) : base(filePathProvider, fileNameProvider, validator, fileProcessingStatistics, logger)
		{
		}

		protected override ExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			return PhysicalFileExportRequest.CreateRequestForPdf(artifact, destinationLocation);
		}

		protected override bool IsFileToExport(ObjectExportInfo artifact)
		{
			return artifact.HasPdf;
		}

		protected override void SaveDestinationLocation(ObjectExportInfo artifact, string destinationLocation)
		{
			artifact.PdfDestinationLocation = destinationLocation;
		}

		protected override string GetFileName(ObjectExportInfo artifact)
		{
			return this.FileNameProvider.GetPdfName(artifact);
		}
	}
}