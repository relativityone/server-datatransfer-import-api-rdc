using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class NativeFileExportRequestBuilder : FileExportRequestBuilder
	{
		public NativeFileExportRequestBuilder(NativeFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, ExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics, ILog logger) : base(filePathProvider, fileNameProvider, validator, fileProcessingStatistics, logger)
		{
		}

		protected override FileExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation)
		{
			return new NativeFileExportRequest(artifact, destinationLocation);
		}

		protected override bool IsFileToExport(ObjectExportInfo artifact)
		{
			return !string.IsNullOrWhiteSpace(artifact.NativeFileGuid);
		}
	}
}