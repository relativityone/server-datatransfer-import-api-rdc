namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Logging;

	public class LongTextEncodingConverter2 : IFileDownloadSubscriber
	{
		private List<Task> _conversionTasks = new List<Task>();

		private readonly LongTextRepository _longTextRepository;
		private readonly IFileEncodingConverter _fileEncodingConverter;
		private readonly ILog _logger;
		private readonly IStatus _status;

		private CancellationToken _cancellationToken;
		private IDisposable _fileDownloadedSubscriber;

		public LongTextEncodingConverter2(
			LongTextRepository longTextRepository,
			IFileEncodingConverter fileEncodingConverter,
			IStatus staus,
			ILog logger)
		{
			_longTextRepository = longTextRepository.ThrowIfNull(nameof(longTextRepository));
			_fileEncodingConverter = fileEncodingConverter.ThrowIfNull(nameof(fileEncodingConverter));
			_logger = logger.ThrowIfNull(nameof(logger));
			_status = staus.ThrowIfNull(nameof(staus));
		}

		public void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer, CancellationToken token)
		{
			fileTransferProducer.ThrowIfNull(nameof(fileTransferProducer));

			_cancellationToken = token;

			this._fileDownloadedSubscriber = fileTransferProducer.FileDownloaded.Subscribe(this.AddForConversion);
		}

		public async Task WaitForConversionCompletion()
		{
			_logger.LogVerbose("Waiting on large text conversion tasks to complete...");
			await Task.WhenAll(_conversionTasks);
			_logger.LogVerbose("Clearing conversion tasks list...");
			_conversionTasks.Clear();
		}

		private void AddForConversion(string longTextFileName)
		{
			try
			{
				this._logger.LogVerbose(
					"Preparing to check whether the '{LongTextFileName}' file requires an encoding conversion...", longTextFileName);
				LongText longText = this.GetLongTextForFile(longTextFileName);
				if (this.ConversionRequired(longText))
				{
					this._logger.LogVerbose(
						"Long text encoding conversion required for file {LongTextFileName}.",
						longTextFileName);
					_conversionTasks.Add(Task.Run(() => ConvertLongText(longText), this._cancellationToken));
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
				_logger.LogError(ex, "Encoding conversion task creation issue for {longTextFileName} file", longTextFileName);
				_status.WriteError($"Encoding conversion task creation issue for {longTextFileName} file");
			}
		}

		private void ConvertLongText(LongText longText)
		{
			try
			{
				this.ConvertLongTextFile(longText);
			}
			catch (Exception ex)
			{
				_status.WriteError($"Encoding conversion task creation issue for {longText.Location} file");
				this._logger.LogError(ex, "The error happened when converting {longTextFileName} file", longText.Location);
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