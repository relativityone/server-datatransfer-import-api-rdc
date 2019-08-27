namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Logging;

	public class LongTextEncodingConverter : IFileDownloadSubscriber
	{
		private readonly BlockingCollection<string> _longTextFilesToConvert;
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileEncodingConverter _fileEncodingConverter;
		private readonly ILog _logger;
		private readonly CancellationToken _cancellationToken;

		private IDisposable _fileDownloadedSubscriber;
		private IDisposable _fileDownloadCompletedSubscriber;

		private Task _conversionTask;

		public LongTextEncodingConverter(
			LongTextRepository longTextRepository,
			IFileEncodingConverter fileEncodingConverter,
			ILog logger,
			CancellationToken cancellationToken)
		{
			_longTextRepository = longTextRepository.ThrowIfNull(nameof(longTextRepository));
			_fileEncodingConverter = fileEncodingConverter.ThrowIfNull(nameof(fileEncodingConverter));
			_logger = logger.ThrowIfNull(nameof(logger));
			_cancellationToken = cancellationToken;
			_longTextFilesToConvert = new BlockingCollection<string>();
		}

		public int Count => _longTextFilesToConvert.Count;

		public async Task WaitForConversionCompletion()
		{
			_logger.LogVerbose("Waiting for the long text encoding conversion to complete.");
			await this._conversionTask.ConfigureAwait(false);
			_logger.LogVerbose("Successfully awaited the long text encoding conversion to complete.");
		}

		public void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer)
		{
			fileTransferProducer.ThrowIfNull(nameof(fileTransferProducer));

			this._fileDownloadedSubscriber = fileTransferProducer.FileDownloaded.Subscribe(this.AddForConversion);
			this._fileDownloadCompletedSubscriber = fileTransferProducer.FileDownloadCompleted.Subscribe(this.CompleteConversion);

			this._conversionTask = Task.Run(() => this.ConvertLongTextFiles(), this._cancellationToken);
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
				_logger.LogVerbose(
					"Preparing to add the '{fileName}' long text file to the queue...",
					fileName);
				_longTextFilesToConvert.Add(fileName, this._cancellationToken);
				_logger.LogVerbose(
					"Successfully added the '{fileName}' long text file to the queue.",
					fileName);
			}
			catch (InvalidOperationException e2)
			{
				_logger.LogError(
					e2,
					"The long text encoding converter received a transfer successful progress event but the blocking collection has already been marked as completed. This exception suggests either a logic or task switch context issue.");
				throw;
			}
		}

		private void ConvertLongTextFiles()
		{
			try
			{
				_logger.LogVerbose("Preparing to start the long text file queue...");
				int totalConvertedTextFiles = 0;
				while (_longTextFilesToConvert.TryTake(out string longTextFileName, Timeout.Infinite, _cancellationToken))
				{
					_logger.LogVerbose(
						"Preparing to check whether the '{LongTextFileName}' file requires an encoding conversion...",
						longTextFileName);
					LongText longText = this.GetLongTextForFile(longTextFileName);
					if (this.ConversionRequired(longText))
					{
						_logger.LogVerbose(
							"Long text encoding conversion required for file {LongTextFileName}.",
							longTextFileName);
						this.ConvertLongTextFile(longText);
					}
					else
					{
						_logger.LogVerbose(
							"Long text encoding conversion NOT required for file {LongTextFileName}.",
							longTextFileName);
					}

					totalConvertedTextFiles++;
				}

				_logger.LogVerbose(
					"Successfully awaited the long text file queue. Total conversions: {totalConvertedTextFiles}.",
					totalConvertedTextFiles);
			}
			catch (OperationCanceledException e)
			{
				_logger.LogInformation(e, "LongText encoding conversion canceled.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to convert long text file.");
				throw;
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
			this._fileDownloadCompletedSubscriber?.Dispose();
		}
	}
}