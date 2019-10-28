namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.Logging;

	public class LongTextEncodingConverter2 : IFileDownloadSubscriber
	{
		private readonly BlockingCollection<string> _longTextFilesToConvert;
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileEncodingConverter _fileEncodingConverter;
		private readonly ILog _logger;

		private readonly IErrorFileWriter _errorFileWriter;

		private CancellationToken _cancellationToken;
		private IDisposable _fileDownloadedSubscriber;

		public LongTextEncodingConverter2(
			LongTextRepository longTextRepository,
			IFileEncodingConverter fileEncodingConverter,
			IErrorFileWriter errorFileWriter,
			ILog logger)
		{
			_longTextRepository = longTextRepository.ThrowIfNull(nameof(longTextRepository));
			_fileEncodingConverter = fileEncodingConverter.ThrowIfNull(nameof(fileEncodingConverter));
			_logger = logger.ThrowIfNull(nameof(logger));
			_errorFileWriter = errorFileWriter.ThrowIfNull(nameof(errorFileWriter));
		}

		public void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer, CancellationToken token)
		{
			fileTransferProducer.ThrowIfNull(nameof(fileTransferProducer));

			_cancellationToken = token;

			this._fileDownloadedSubscriber = fileTransferProducer.FileDownloaded.Subscribe(this.AddForConversion);
		}

		private void CompleteConversion(bool anyFileToConvert)
		{
			_logger.LogVerbose("Preparing to mark the long text encoding conversion queue complete...");
			_logger.LogVerbose("Any files to convert: {anyFileToConvert} ", anyFileToConvert);
			_longTextFilesToConvert.CompleteAdding();
			_logger.LogVerbose("Successfully marked the long text encoding conversion queue complete.");
		}

		private void AddForConversion(string fileName)
		{
			try
			{
				Task.Run(() => ConvertLongText(fileName), this._cancellationToken);
			}
			catch (OperationCanceledException ex)
			{
				this._logger.LogInformation(ex, "The cancellation operation has been requested by the user");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Encoding conversion task creation issue for {fileName} file");
				this._errorFileWriter.Write(
					ErrorFileWriter.ExportFileType.Generic,
					null,
					fileName,
					$"Encoding conversion task creation issue for {fileName} file");
			}
		}

		private void ConvertLongText(string longTextFileName)
		{
			try
			{
				this._logger.LogVerbose(
					"Preparing to check whether the '{LongTextFileName}' file requires an encoding conversion...",
					longTextFileName);
				LongText longText = this.GetLongTextForFile(longTextFileName);
				if (this.ConversionRequired(longText))
				{
					this._logger.LogVerbose(
						"Long text encoding conversion required for file {LongTextFileName}.",
						longTextFileName);
					this.ConvertLongTextFile(longText);
				}
				else
				{
					this._logger.LogVerbose(
						"Long text encoding conversion NOT required for file {LongTextFileName}.",
						longTextFileName);
				}
			}
			catch (OperationCanceledException ex)
			{
				this._logger.LogInformation(ex, "The cancellation operation has been requested by the user");
			}
			catch (Exception ex)
			{
				this._errorFileWriter.Write(
					ErrorFileWriter.ExportFileType.Generic,
					null,
					longTextFileName,
					$"Encoding conversion task creation issue for {longTextFileName} file");
				this._logger.LogError(ex, "The error happened when converting {0} file", longTextFileName);
			}
		}

		private LongText GetLongTextForFile(string longTextFileName)
		{
			foreach (LongText longText in _longTextRepository.GetLongTexts()
				.Where(x => !string.IsNullOrEmpty(x.Location)))
			{
				string fileName = System.IO.Path.GetFileName(longText.Location);
				if (string.Compare(fileName, longTextFileName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return longText;
				}
			}

			_logger.LogError("Failed to find the LongText file {LongTextFileName} in the repository.", longTextFileName);
			throw new ArgumentException($"The long text file {longTextFileName} cannot be converted because it doesn't exist within the export request.");
		}

		private bool ConversionRequired(LongText longText)
		{
			return !longText.SourceEncoding.Equals(longText.DestinationEncoding);
		}

		private void ConvertLongTextFile(LongText longText)
		{
			_logger.LogVerbose(
				"Preparing to convert LongText file {LongTextFile} from {SourceEncoding} to {DestinationEncoding}.",
				longText.Location,
				longText.SourceEncoding,
				longText.DestinationEncoding);
			_fileEncodingConverter.Convert(
				longText.Location,
				longText.SourceEncoding,
				longText.DestinationEncoding,
				_cancellationToken);
			_logger.LogVerbose(
				"Successfully converted LongText file {LongTextFile} from {SourceEncoding} to {DestinationEncoding}.",
				longText.Location,
				longText.SourceEncoding,
				longText.DestinationEncoding);
			longText.SourceEncoding = longText.DestinationEncoding;
		}

		public void Dispose()
		{
			this._fileDownloadedSubscriber?.Dispose();
		}
	}
}