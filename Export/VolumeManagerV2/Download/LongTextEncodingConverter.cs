using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class LongTextEncodingConverter : IDisposable, ILongTextEncodingConverter
	{
		private Task _conversionTask;

		private readonly BlockingCollection<string> _longTextFilesToConvert;
		private readonly ExportFile _exportSettings;
		private readonly LongTextRepository _longTextRepository;
		private readonly FileEncodingConverter _fileEncodingConverter;
		private readonly ILog _logger;
		private readonly CancellationToken _cancellationToken;

		public LongTextEncodingConverter(ExportFile exportSettings, LongTextRepository longTextRepository, FileEncodingConverter fileEncodingConverter, ILog logger,
			CancellationToken cancellationToken)
		{
			_exportSettings = exportSettings;
			_longTextRepository = longTextRepository;
			_fileEncodingConverter = fileEncodingConverter;
			_logger = logger;
			_cancellationToken = cancellationToken;

			_longTextFilesToConvert = new BlockingCollection<string>();
		}

		private void OnTapiProgress(object sender, TapiProgressEventArgs e)
		{
			_longTextFilesToConvert.Add(e.FileName);
		}

		public void StartListening(ITapiBridge tapiBridge)
		{
			_logger.LogVerbose("Start conversion task.");
			_conversionTask = Task.Run(() => ConvertLongTextFiles(), _cancellationToken);
			tapiBridge.TapiProgress += OnTapiProgress;
		}

		public void StopListening(ITapiBridge tapiBridge)
		{
			_logger.LogVerbose("Stop listening for new files to convert.");
			tapiBridge.TapiProgress -= OnTapiProgress;
			_longTextFilesToConvert.CompleteAdding();
		}

		public void WaitForConversionCompletion()
		{
			_logger.LogVerbose("Waiting for conversion to complete.");
			_conversionTask.ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private void ConvertLongTextFiles()
		{
			try
			{
				string longTextFile;
				while (_longTextFilesToConvert.TryTake(out longTextFile, Timeout.Infinite, _cancellationToken))
				{
					_logger.LogVerbose("New item in conversion queue {file}. Proceeding.", longTextFile);
					LongText longText = GetLongTextForFile(longTextFile);
					if (ConversionRequired(longText))
					{
						_logger.LogVerbose("Encoding conversion required for file {file}.", longTextFile);
						ConvertLongTextFile(longText);
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
				string fileName = new System.IO.FileInfo(longText.Location).Name;
				if (fileName == longTextFile)
				{
					return longText;
				}
			}

			_logger.LogError("Could not found LongText for file {file}.", longTextFile);
			throw new ArgumentException($"Could not found LongText for file {longTextFile}.");
		}

		private bool ConversionRequired(LongText longText)
		{
			return !longText.SourceEncoding.Equals(longText.DestinationEncoding);
		}

		private void ConvertLongTextFile(LongText longText)
		{
			_logger.LogVerbose("Converting LongText file {file} from {sourceEncoding} to {destinationEncoding}.", longText.Location, longText.SourceEncoding, longText.DestinationEncoding);

			_fileEncodingConverter.Convert(longText.Location, longText.SourceEncoding, _exportSettings.TextFileEncoding, _cancellationToken);

			if (_cancellationToken.IsCancellationRequested)
			{
				return;
			}

			longText.SourceEncoding = longText.DestinationEncoding;
		}

		public void Dispose()
		{
			_longTextFilesToConvert?.Dispose();
		}
	}
}