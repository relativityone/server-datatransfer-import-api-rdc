
using kCura.WinEDDS.Exporters;
using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
using Relativity.DataExchange.Io;
using Relativity.DataExchange.Logger;
using Relativity.DataExchange.Resources;
using System.Globalization;
using System.Threading;

namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using Relativity.Logging;

	public class PdfFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly IAppSettings _settings;
		private readonly ILog _logger;

		public PdfFileBatchValidator(IErrorFileWriter errorFileWriter, IFile fileWrapper, ILog logger)
			: this(errorFileWriter, fileWrapper, AppSettings.Instance, logger)
		{
		}

		public PdfFileBatchValidator(
			IErrorFileWriter errorFileWriter,
			IFile fileWrapper,
			IAppSettings settings,
			ILog logger)
		{
			this._errorFileWriter = errorFileWriter.ThrowIfNull(nameof(errorFileWriter));
			this._fileWrapper = fileWrapper.ThrowIfNull(nameof(fileWrapper));
			this._settings = settings.ThrowIfNull(nameof(settings));
			this._logger = logger.ThrowIfNull(nameof(logger));
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			foreach (var artifact in artifacts)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				this.ValidatePdfsForArtifact(artifact);
			}
		}

		private void ValidatePdfsForArtifact(ObjectExportInfo artifact)
		{
			if (string.IsNullOrWhiteSpace(artifact.PdfDestinationLocation))
			{
				return;
			}

			bool fileExists = this._fileWrapper.Exists(artifact.PdfDestinationLocation);
			if (!fileExists || this._fileWrapper.GetFileSize(artifact.PdfDestinationLocation) == 0)
			{
				if (fileExists && !this._settings.CreateErrorForEmptyPdfFile)
				{
					this._logger.LogVerbose(
						"PDF file {File} contains zero bytes for artifact {ArtifactId} but the export is configured to skip creating an error.",
						artifact.PdfDestinationLocation.Secure(),
						artifact.ArtifactID);
					return;
				}

				string errorMessage = string.Format(
					CultureInfo.CurrentCulture,
					fileExists ? ExportStrings.FileValidationZeroByteFile : ExportStrings.FileValidationFileMissing,
					artifact.ArtifactID);
				if (string.IsNullOrWhiteSpace(artifact.PdfSourceLocation))
				{
					errorMessage = string.Format(
						CultureInfo.CurrentCulture,
						ExportStrings.FileValidationEmptyRemoteSourcePath,
						artifact.ArtifactID);
					_logger.LogError(
						"PDF file remote source path is null or whitespace for PDF artifact {ArtifactId} and indicates a problem with the artifact data.",
						artifact.ArtifactID);
				}
				else
				{
					this._logger.LogError(
						fileExists
							? "PDF file contains zero bytes for artifact {ArtifactId}."
							: "PDF file is missing for artifact {ArtifactId}.",
						artifact.ArtifactID);
				}

				_errorFileWriter.Write(
					ErrorFileWriter.ExportFileType.Pdf,
					artifact,
					artifact.PdfDestinationLocation,
					errorMessage);
			}
		}
	}
}
