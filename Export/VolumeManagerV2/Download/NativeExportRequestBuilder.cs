using System.Collections.Generic;
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class NativeExportRequestBuilder : IExportRequestBuilder
	{
		private readonly ExportFile _exportSettings;
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFileHelper _fileHelper;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public NativeExportRequestBuilder(ExportFile exportSettings, NativeFilePathProvider filePathProvider, IFileHelper fileHelper, IFileNameProvider fileNameProvider, IStatus status,
			ILog logger)
		{
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_fileHelper = fileHelper;
			_fileNameProvider = fileNameProvider;
			_status = status;
			_logger = logger;
		}

		public IEnumerable<ExportRequest> Create(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Creating native file ExportRequest for artifact {artifactId}.", artifact.ArtifactID);
			string destinationLocation = GetExportDestinationLocation(artifact);

			if (!CanExport(destinationLocation))
			{
				return Enumerable.Empty<ExportRequest>();
			}

			_logger.LogVerbose("Native file for artifact {artifactId} will be export to {destinationLocation}.", artifact.ArtifactID, destinationLocation);
			artifact.NativeTempLocation = destinationLocation;

			ExportRequest exportRequest = new ExportRequest(artifact, destinationLocation);
			return new List<ExportRequest> {exportRequest};
		}

		private string GetExportDestinationLocation(ObjectExportInfo artifact)
		{
			string fileName = _fileNameProvider.GetName(artifact);

			return _filePathProvider.GetPathForFile(fileName);
		}

		private bool CanExport(string destinationLocation)
		{
			if (_fileHelper.Exists(destinationLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_logger.LogVerbose($"Overwriting document {destinationLocation}. Removing already existing file.");
					_fileHelper.Delete(destinationLocation);
					_status.WriteStatusLine(EventType.Status, $"Overwriting document {destinationLocation}.", false);
				}
				else
				{
					_logger.LogVerbose($"{destinationLocation} already exists. Skipping file export.");
					_status.WriteWarning($"{destinationLocation} already exists. Skipping file export.");
					return false;
				}
			}
			return true;
		}
	}
}