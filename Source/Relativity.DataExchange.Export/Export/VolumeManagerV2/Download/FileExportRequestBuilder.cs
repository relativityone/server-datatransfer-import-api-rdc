namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	public abstract class ExportRequestBuilder : IExportRequestBuilder
	{
		protected readonly IFileNameProvider FileNameProvider;
		private readonly IFilePathProvider _filePathProvider;
		private readonly IExportFileValidator _validator;
		private readonly IFileProcessingStatistics _fileProcessingStatistics;
		private readonly ILog _logger;

		protected ExportRequestBuilder(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, IExportFileValidator validator,
		                               IFileProcessingStatistics fileProcessingStatistics, ILog logger)
		{
			_filePathProvider = filePathProvider;
			FileNameProvider = fileNameProvider;
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

			string destinationLocation = RetrieveFileNameAndDestinationLocation(artifact);
			
			string warningInCaseOfOverwriting = $"Overwriting document {destinationLocation}.";
			if (!_validator.CanExport(destinationLocation, warningInCaseOfOverwriting))
			{
				_logger.LogVerbose("File {file} already exists - updating statistics.", destinationLocation.Secure());
				_fileProcessingStatistics.UpdateStatisticsForFile(destinationLocation);
				return new List<ExportRequest>();
			}

			_logger.LogVerbose("Native file for artifact {artifactId} will be export to {destinationLocation}.", artifact.ArtifactID, destinationLocation.Secure());

			ExportRequest exportRequest = CreateExportRequest(artifact, destinationLocation);
			return exportRequest.InList();
		}

		protected virtual string RetrieveFileNameAndDestinationLocation(ObjectExportInfo artifact)
		{
			string fileName = GetFileName(artifact);
			string destinationLocation = GetExportDestinationLocation(fileName, artifact.ArtifactID);
			SaveDestinationLocation(artifact, destinationLocation);
			return destinationLocation;
		}

		protected abstract ExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation);
		
		protected abstract bool IsFileToExport(ObjectExportInfo artifact);

		protected virtual string GetFileName(ObjectExportInfo artifact)
		{
			return FileNameProvider.GetName(artifact);
		}

		protected virtual void SaveDestinationLocation(ObjectExportInfo artifact, string destinationLocation)
		{
			artifact.NativeTempLocation = destinationLocation;
		}

		private string GetExportDestinationLocation(string fileName, int artifactId)
		{
			return _filePathProvider.GetPathForFile(fileName, artifactId);
		}
	}
}