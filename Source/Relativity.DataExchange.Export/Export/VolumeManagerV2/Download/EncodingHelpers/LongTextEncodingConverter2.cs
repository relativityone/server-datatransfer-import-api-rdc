namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.InteropServices;
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

		private readonly IErrorFileWriter errorFileWriter;

		private readonly CancellationToken _cancellationToken;
		private List<string> initialFilesToConvert;
		private IDisposable _fileDownloadedSubscriber;
		private IDisposable _fileDownloadCompletedSubscriber;

		private Task _conversionTask;

		private TaskCompletionSource<bool> tcs;

		public LongTextEncodingConverter2(
			LongTextRepository longTextRepository,
			IFileEncodingConverter fileEncodingConverter,
			IErrorFileWriter errorFileWriter,
			ILog logger,
			CancellationToken cancellationToken)
		{
			_longTextRepository = longTextRepository.ThrowIfNull(nameof(longTextRepository));
			_fileEncodingConverter = fileEncodingConverter.ThrowIfNull(nameof(fileEncodingConverter));
			_logger = logger.ThrowIfNull(nameof(logger));
			this.errorFileWriter = errorFileWriter.ThrowIfNull(nameof(errorFileWriter)); ;
			_cancellationToken = cancellationToken;
			_longTextFilesToConvert = new BlockingCollection<string>();

			var exportRequests = this._longTextRepository.GetExportRequests();
			exportRequests.ThrowIfNull(nameof(exportRequests));
			this.initialFilesToConvert = new List<string>(exportRequests.Select(item => Path.GetFileName(item.DestinationLocation)));

			tcs = new TaskCompletionSource<bool>(cancellationToken);
		}

		public int Count => _longTextFilesToConvert.Count;

		public async Task WaitForConversionCompletion()
		{
			await this.tcs.Task;
		}

		public void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer)
		{
			fileTransferProducer.ThrowIfNull(nameof(fileTransferProducer));

			this._fileDownloadedSubscriber = fileTransferProducer
				.FileDownloaded
				.Subscribe(this.AddForConversion);
			// this._fileDownloadCompletedSubscriber = fileTransferProducer.FileDownloadCompleted.Subscribe(this.CompleteConversion);

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
				_longTextFilesToConvert.Add(fileName);
			}
			// We should never re-thrown exception from here as it will cause not handled exception
			// We have two practical options that would be triggered
			// 1) Cancellation - this will end the entire job
			// 2) InvalidOperationException - issue with adding the item to blocking collection that is more like unexpected error
			catch (InvalidOperationException ex)
			{
				this._logger.LogError(ex, "The long text encoding converter received a transfer successful progress event but the blocking collection has already been marked as completed. This exception suggests either a logic or task switch context issue.");
			}
			catch (OperationCanceledException ex)
			{
				this._logger.LogInformation(ex, "The cancellation operation has been requested by the user");
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "The error happened when converting {0} file", fileName);
			}
		}

		private void ConvertLongTextFiles()
		{
			try
			{
				_logger.LogVerbose("Preparing to start the long text file queue...");
				if (!this.initialFilesToConvert.Any())
				{
					_logger.LogDebug("No request to process long text files in the batch");
					return;
				}
				int totalConvertedTextFiles = 0;
				while (_longTextFilesToConvert.TryTake(
					out string longTextFileName,
					Timeout.Infinite,
					_cancellationToken))
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
					if (NothingToProcess(longTextFileName))
					{
						break;
					}
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
			finally
			{
				tcs.SetResult(true);
			}
		}

		private bool NothingToProcess(string longTextFileName)
		{
			if (initialFilesToConvert.Remove(longTextFileName))
			{
				return !this.initialFilesToConvert.Any();
			}
			// That should never happened
			errorFileWriter.Write(ErrorFileWriter.ExportFileType.Generic, null, String.Empty, 
					$"Can not find file: {longTextFileName} in the requested long text file list for encoding conversion. Skipping conversion for the whole batch");
			return true;
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