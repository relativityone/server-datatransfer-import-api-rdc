using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class NativeExportRequestBuilder : IFileExportRequestBuilder
	{
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly ExportFileValidator _validator;
		private readonly ILog _logger;

		public NativeExportRequestBuilder(NativeFilePathProvider filePathProvider, IFileNameProvider fileNameProvider, ExportFileValidator validator, ILog logger)
		{
			_filePathProvider = filePathProvider;
			_fileNameProvider = fileNameProvider;
			_logger = logger;
			_validator = validator;
		}

		public IList<FileExportRequest> Create(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Creating native file ExportRequest for artifact {artifactId}.", artifact.ArtifactID);
			string destinationLocation = GetExportDestinationLocation(artifact);

			string warningInCaseOfOverwriting = $"Overwriting document {destinationLocation}.";
			if (!_validator.CanExport(destinationLocation, warningInCaseOfOverwriting))
			{
				return new List<FileExportRequest>();
			}

			_logger.LogVerbose("Native file for artifact {artifactId} will be export to {destinationLocation}.", artifact.ArtifactID, destinationLocation);
			artifact.NativeTempLocation = destinationLocation;

			FileExportRequest exportRequest = new FileExportRequest(artifact, destinationLocation);
			return exportRequest.InList();
		}

		private string GetExportDestinationLocation(ObjectExportInfo artifact)
		{
			string fileName = _fileNameProvider.GetName(artifact);

			return _filePathProvider.GetPathForFile(fileName);
		}
	}
}