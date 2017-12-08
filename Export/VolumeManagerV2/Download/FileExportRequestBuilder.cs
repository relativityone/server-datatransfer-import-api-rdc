using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public abstract class FileExportRequestBuilder : IFileExportRequestBuilder
	{
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly ExportFileValidator _validator;
		private readonly ILog _logger;

		protected FileExportRequestBuilder(IFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, ExportFileValidator validator, ILog logger)
		{
			_filePathProvider = filePathProvider;
			_fileNameProvider = fileNameProvider;
			_validator = validator;
			_logger = logger;
		}

		public IList<FileExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			if (!IsFileToExport(artifact))
			{
				_logger.LogVerbose("No native file to export for artifact {artifactId}.", artifact.ArtifactID);
				return Enumerable.Empty<FileExportRequest>().ToList();
			}
			_logger.LogVerbose("Creating native file ExportRequest for artifact {artifactId}.", artifact.ArtifactID);

			if (cancellationToken.IsCancellationRequested)
			{
				return Enumerable.Empty<FileExportRequest>().ToList();
			}

			string destinationLocation = GetExportDestinationLocation(artifact);

			string warningInCaseOfOverwriting = $"Overwriting document {destinationLocation}.";
			if (!_validator.CanExport(destinationLocation, warningInCaseOfOverwriting))
			{
				return new List<FileExportRequest>();
			}

			_logger.LogVerbose("Native file for artifact {artifactId} will be export to {destinationLocation}.", artifact.ArtifactID, destinationLocation);
			artifact.NativeTempLocation = destinationLocation;

			FileExportRequest exportRequest = CreateExportRequest(artifact, destinationLocation);
			return exportRequest.InList();
		}

		protected abstract FileExportRequest CreateExportRequest(ObjectExportInfo artifact, string destinationLocation);

		protected abstract bool IsFileToExport(ObjectExportInfo artifact);

		private string GetExportDestinationLocation(ObjectExportInfo artifact)
		{
			string fileName = _fileNameProvider.GetName(artifact);

			return _filePathProvider.GetPathForFile(fileName);
		}
	}
}