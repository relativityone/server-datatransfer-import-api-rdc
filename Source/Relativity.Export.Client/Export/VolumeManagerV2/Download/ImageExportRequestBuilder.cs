using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.Core;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class ImageExportRequestBuilder : IExportRequestBuilder
	{
		private readonly IFilePathProvider _filePathProvider;
		private readonly IExportFileValidator _validator;
		private readonly IFileProcessingStatistics _fileProcessingStatistics;
		private readonly ILog _logger;

		public ImageExportRequestBuilder(ImageFilePathProvider filePathProvider, IExportFileValidator validator, ILog logger, IFileProcessingStatistics fileProcessingStatistics) : this(
			(IFilePathProvider) filePathProvider, validator, logger, fileProcessingStatistics)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="filePathProvider"></param>
		/// <param name="validator"></param>
		/// <param name="logger"></param>
		/// <param name="fileProcessingStatistics"></param>
		[DoNotSelect]
		public ImageExportRequestBuilder(IFilePathProvider filePathProvider, IExportFileValidator validator, ILog logger, IFileProcessingStatistics fileProcessingStatistics)
		{
			_filePathProvider = filePathProvider;
			_validator = validator;
			_logger = logger;
			_fileProcessingStatistics = fileProcessingStatistics;
		}

		public IList<ExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			var fileExportRequests = new List<ExportRequest>();
			_logger.LogVerbose("Creating image files ExportRequests for artifact {artifactId}.", artifact.ArtifactID);
			foreach (var image in artifact.Images.Cast<ImageExportInfo>())
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Enumerable.Empty<ExportRequest>().ToList();
				}

				ExportRequest exportRequest;
				if (TryCreate(artifact, image, out exportRequest))
				{
					fileExportRequests.Add(exportRequest);
				}
			}

			return fileExportRequests;
		}

		private bool TryCreate(ObjectExportInfo artifact, ImageExportInfo image, out ExportRequest exportRequest)
		{
			if (string.IsNullOrWhiteSpace(image.FileGuid))
			{
				_logger.LogInformation("Image {imageId} has no GUID so assuming there is nothing to download.", image.ArtifactID);
				exportRequest = null;
				return false;
			}

			_logger.LogVerbose("Creating image file ExportRequest for image {image}.", image.FileName);
			string destinationLocation = GetExportDestinationLocation(artifact, image);
			image.TempLocation = destinationLocation;

			string warningInCaseOfOverwriting = $"Overwriting image for {image.BatesNumber}.";

			if (!_validator.CanExport(destinationLocation, warningInCaseOfOverwriting))
			{
				_logger.LogVerbose("File {file} already exists - updating statistics.", destinationLocation);
				_fileProcessingStatistics.UpdateStatisticsForFile(destinationLocation);
				exportRequest = null;
				return false;
			}

			_logger.LogVerbose("Image file will be export to {destinationLocation}.", destinationLocation);

			exportRequest = new PhysicalFileExportRequest(image, destinationLocation);
			return true;
		}

		private string GetExportDestinationLocation(ObjectExportInfo artifact, ImageExportInfo image)
		{
			string fileName = image.FileName;
			return _filePathProvider.GetPathForFile(fileName, artifact.ArtifactID);
		}	
	}
}