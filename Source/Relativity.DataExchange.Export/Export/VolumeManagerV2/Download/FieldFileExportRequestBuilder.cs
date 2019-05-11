namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using Castle.Core;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	public class FieldFileExportRequestBuilder : ExportRequestBuilder
	{
		private readonly FieldFileExportRequestFactory _requestFactory;

		public FieldFileExportRequestBuilder(NativeFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics, ILog logger, FieldFileExportRequestFactory requestFactory) : base(filePathProvider, fileNameProvider, validator,
			fileProcessingStatistics, logger)
		{
			_requestFactory = requestFactory;
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="filePathProvider"></param>
		/// <param name="fileNameProvider"></param>
		/// <param name="validator"></param>
		/// <param name="fileProcessingStatistics"></param>
		/// <param name="logger"></param>
		/// <param name="requestFactory"></param>
		[DoNotSelect]
		public FieldFileExportRequestBuilder(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics, ILog logger, FieldFileExportRequestFactory requestFactory) : base(filePathProvider, fileNameProvider, validator,
			fileProcessingStatistics, logger)
		{
			_requestFactory = requestFactory;
		}

		protected override ExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			return _requestFactory.Create(artifact, destinationLocation);
		}

		protected override bool IsFileToExport(ObjectExportInfo artifact)
		{
			return artifact.FileID > 0 || !string.IsNullOrWhiteSpace(artifact.NativeSourceLocation);
		}
	}
}