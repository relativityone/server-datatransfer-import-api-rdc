using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class ExportRequestBuilder : IExportRequestBuilder
	{
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly IExportFileValidator _validator;
		private readonly IFileProcessingStatistics _fileProcessingStatistics;
		private readonly ILog _logger;

		protected ExportRequestBuilder(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator,
			IFileProcessingStatistics fileProcessingStatistics, ILog logger)
		{
			_filePathProvider = filePathProvider;
			_fileNameProvider = fileNameProvider;
			_validator = validator;
			_fileProcessingStatistics = fileProcessingStatistics;
			_logger = logger;
		}

		public IList<ExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			if (!IsFileToExport(artifact))
			{
				_logger.LogVerbose("No native file to export for artifact {artifactId}.", artifact.ArtifactID);
				return Enumerable.Empty<ExportRequest>().ToList();
			}

			_logger.LogVerbose("Creating native file ExportRequest for artifact {artifactId}.", artifact.ArtifactID);

			if (cancellationToken.IsCancellationRequested)
			{
				return Enumerable.Empty<ExportRequest>().ToList();
			}

			string destinationLocation = GetExportDestinationLocation(artifact);
			artifact.NativeTempLocation = destinationLocation;

			string warningInCaseOfOverwriting = $"Overwriting document {destinationLocation}.";
			if (!_validator.CanExport(destinationLocation, warningInCaseOfOverwriting))
			{
				_logger.LogVerbose("File {file} already exists - updating statistics.", destinationLocation);
				_fileProcessingStatistics.UpdateStatisticsForFile(destinationLocation);
				return new List<ExportRequest>();
			}

			_logger.LogVerbose("Native file for artifact {artifactId} will be export to {destinationLocation}.", artifact.ArtifactID, destinationLocation);

			ExportRequest exportRequest = CreateExportRequest(artifact, destinationLocation);
			return exportRequest.InList();
		}

		protected abstract ExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation);

		protected abstract bool IsFileToExport(ObjectExportInfo artifact);

		private string GetExportDestinationLocation(ObjectExportInfo artifact)
		{
			string fileName = _fileNameProvider.GetName(artifact);

			return _filePathProvider.GetPathForFile(fileName);
		}
	}
}