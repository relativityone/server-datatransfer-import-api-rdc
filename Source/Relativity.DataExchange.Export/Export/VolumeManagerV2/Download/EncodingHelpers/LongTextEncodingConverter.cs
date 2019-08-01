namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Logging;

	public class LongTextEncodingConverter : IDisposable, ILongTextEncodingConverter
	{
		private readonly BlockingCollection<string> _longTextFilesToConvert;
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileEncodingConverter _fileEncodingConverter;
		private readonly ILog _logger;
		private readonly CancellationToken _cancellationToken;
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

		public void StartListening(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose(
				"Attached tapi bridge {TapiBridgeInstanceId} to the long text encoding converter.",
				tapiBridge.InstanceId);
			_conversionTask = Task.Run(() => this.ConvertLongTextFiles(), _cancellationToken);
			tapiBridge.TapiProgress += this.OnTapiProgress;
		}

		public void StopListening(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose(
				"Detached tapi bridge {TapiBridgeInstanceId} from the long text encoding converter.",
				tapiBridge.InstanceId);
			tapiBridge.TapiProgress -= this.OnTapiProgress;
			_longTextFilesToConvert.CompleteAdding();
		}

		private void OnTapiProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose(
				"Long text encoding conversion progress event for file {fileName} with status {Successful}.",
				e.FileName,
				e.Successful);
			if (e.Successful)
			{
				_longTextFilesToConvert.Add(e.FileName, _cancellationToken);
			}
		}

		public void WaitForConversionCompletion()
		{
			_logger.LogVerbose("Waiting for long text encoding conversion to complete.");
			_conversionTask.ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private void ConvertLongTextFiles()
		{
			try
			{
				string longTextFile;
				while (_longTextFilesToConvert.TryTake(out longTextFile, Timeout.Infinite, _cancellationToken))
				{
					_logger.LogVerbose("New item in long text encoding conversion queue {file}. Proceeding.", longTextFile);
					LongText longText = this.GetLongTextForFile(longTextFile);
					if (this.ConversionRequired(longText))
					{
						_logger.LogVerbose("Long text encoding conversion required for file {file}.", longTextFile);
						this.ConvertLongTextFile(longText);
					}
					else
					{
						_logger.LogVerbose("Long text encoding conversion NOT required for file {file}.", longTextFile);
					}
				}
			}
			catch (OperationCanceledException e)
			{
				_logger.LogError(e, "LongText encoding conversion canceled.");
			}
			catch (ArgumentException)
			{
				throw;
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to convert long text file.");
				throw;
			}
		}

		private LongText GetLongTextForFile(string longTextFile)
		{
			foreach (LongText longText in _longTextRepository.GetLongTexts().Where(x => !string.IsNullOrEmpty(x.Location)))
			{
				string fileName = System.IO.Path.GetFileName(longText.Location);
				if (string.Compare(fileName, longTextFile, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return longText;
				}
			}

			_logger.LogError("Failed to find the LongText file {file} in the repository.", longTextFile);
			throw new ArgumentException($"The long text file {longTextFile} cannot be converted because it doesn't exist within the export request.");
		}

		private bool ConversionRequired(LongText longText)
		{
			return !longText.SourceEncoding.Equals(longText.DestinationEncoding);
		}

		private void ConvertLongTextFile(LongText longText)
		{
			_logger.LogVerbose("Converting LongText file {file} from {sourceEncoding} to {destinationEncoding}.", longText.Location, longText.SourceEncoding, longText.DestinationEncoding);

			_fileEncodingConverter.Convert(longText.Location, longText.SourceEncoding, longText.DestinationEncoding, _cancellationToken);

			longText.SourceEncoding = longText.DestinationEncoding;
		}

		public void Dispose()
		{
			_longTextFilesToConvert?.Dispose();
		}
	}
}