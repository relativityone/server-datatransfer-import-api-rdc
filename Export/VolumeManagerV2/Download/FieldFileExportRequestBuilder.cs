using Castle.Core;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FieldFileExportRequestBuilder : FileExportRequestBuilder
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

		protected override FileExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			return _requestFactory.Create(artifact, destinationLocation);
		}

		protected override bool IsFileToExport(ObjectExportInfo artifact)
		{
			return artifact.FileID > 0 || !string.IsNullOrWhiteSpace(artifact.NativeSourceLocation);
		}
	}
}