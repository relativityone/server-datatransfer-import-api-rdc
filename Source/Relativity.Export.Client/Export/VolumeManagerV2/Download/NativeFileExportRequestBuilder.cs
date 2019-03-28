using Castle.Core;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class NativeFileExportRequestBuilder : ExportRequestBuilder
	{
		public NativeFileExportRequestBuilder(NativeFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics, ILog logger) : base(filePathProvider, fileNameProvider, validator, fileProcessingStatistics, logger)
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
		public NativeFileExportRequestBuilder(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics, ILog logger) : base(filePathProvider, fileNameProvider, validator, fileProcessingStatistics, logger)
		{
		}

		protected override ExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			return new PhysicalFileExportRequest(artifact, destinationLocation);
		}

		protected override bool IsFileToExport(ObjectExportInfo artifact)
		{
			return !string.IsNullOrWhiteSpace(artifact.NativeFileGuid);
		}
	}
}