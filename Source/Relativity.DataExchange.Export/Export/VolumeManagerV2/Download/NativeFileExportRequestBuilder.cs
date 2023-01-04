namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using Castle.Core;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.Logging;

	public class NativeFileExportRequestBuilder : ExportRequestBuilder
	{
		public NativeFileExportRequestBuilder(
			NativeFilePathProvider filePathProvider,
			IFileNameProvider fileNameProvider,
			IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics,
			ILog logger)
			: base(filePathProvider, fileNameProvider, validator, fileProcessingStatistics, logger)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="filePathProvider"></param>
		/// <param name="fileNameProvider"></param>
		/// <param name="validator"></param>
		/// <param name="fileProcessingStatistics"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public NativeFileExportRequestBuilder(
			IFilePathProvider filePathProvider,
			IFileNameProvider fileNameProvider,
			IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics,
			ILog logger)
			: base(filePathProvider, fileNameProvider, validator, fileProcessingStatistics, logger)
		{
		}

		protected override ExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			return PhysicalFileExportRequest.CreateRequestForNative(artifact, destinationLocation);
		}

		protected override bool IsFileToExport(ObjectExportInfo artifact)
		{
			return !string.IsNullOrWhiteSpace(artifact.NativeFileGuid);
		}

		protected override string RetrieveFileNameAndDestinationLocation(ObjectExportInfo artifact)
		{
			artifact.Filename = GetFileName(artifact);
			return base.RetrieveFileNameAndDestinationLocation(artifact);
		}
	}
}