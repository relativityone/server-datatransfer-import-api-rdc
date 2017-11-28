using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	/// <summary>
	///     TODO code duplication in ImageLoadFileWriter
	/// </summary>
	public class LoadFileWriter : IDisposable
	{
		private long _lastStreamWriterPosition;

		private StreamWriter _fileWriter;
		private readonly LoadFileDestinationPath _loadFileDestinationPath;
		private readonly StreamFactory _streamFactory;
		private readonly StatisticsWrapper _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly RetryPolicy _retryPolicy;
		private readonly ILog _logger;

		public LoadFileWriter(StatisticsWrapper statistics, IFileHelper fileHelper, RetryPolicy retryPolicy, StreamFactory streamFactory, LoadFileDestinationPath loadFileDestinationPath,
			ILog logger)
		{
			_statistics = statistics;
			_fileHelper = fileHelper;
			_retryPolicy = retryPolicy;
			_streamFactory = streamFactory;
			_loadFileDestinationPath = loadFileDestinationPath;
			_logger = logger;

			_lastStreamWriterPosition = 0;
		}

		public void Write(ConcurrentDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			if (linesToWrite == null || linesToWrite.Count == 0)
			{
				_logger.LogVerbose("No lines to write to load file - skipping.");
				return;
			}

			_logger.LogVerbose("Writing to load file with retry policy.");
			_retryPolicy.Execute((context, token) =>
			{
				Write(linesToWrite, artifacts, context);
			}, new Context(nameof(LoadFileWriter)), cancellationToken);
		}

		private void Write(ConcurrentDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			//TODO this will open stream for every batch - the question is: is this impacting performance in a way we should change it?
			_fileWriter = _streamFactory.Create(_fileWriter, _lastStreamWriterPosition, _loadFileDestinationPath.Path, _loadFileDestinationPath.Encoding);

			WriteHeaderIfNeeded(linesToWrite);

			WriteArtifacts(linesToWrite, artifacts, context);

			FlushStream();

			SaveStreamPositionAndUpdateStatistics();
		}

		public void WriteHeaderIfNeeded(ConcurrentDictionary<int, ILoadFileEntry> linesToWrite)
		{
			const int headerArtifactID = -1;

			ILoadFileEntry loadFileEntry;
			if (linesToWrite.TryGetValue(headerArtifactID, out loadFileEntry))
			{
				loadFileEntry?.Write(ref _fileWriter);
			}
		}

		private void WriteArtifacts(ConcurrentDictionary<int, ILoadFileEntry> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			foreach (var artifact in artifacts)
			{
				//TODO I don't like this :(
				context[WritersRetryPolicy.CONTEXT_LAST_ARTIFACT_ID_KEY] = artifact.ArtifactID;

				ILoadFileEntry loadFileEntry;
				if (linesToWrite.TryGetValue(artifact.ArtifactID, out loadFileEntry))
				{
					loadFileEntry?.Write(ref _fileWriter);
				}
			}
		}

		private void FlushStream()
		{
			try
			{
				_fileWriter?.Flush();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to flush load file stream.");
				throw new FileWriteException(FileWriteException.DestinationFile.Load, ex);
			}
		}

		private void SaveStreamPositionAndUpdateStatistics()
		{
			if (_fileWriter != null)
			{
				_lastStreamWriterPosition = _fileWriter.BaseStream.Position;
				_statistics.MetadataBytes = _fileHelper.GetFileSize(GetStreamName());
			}
		}

		/// <summary>
		///     TODO it should be the same as _imageLoadFileDestinationPath.Path, right?
		/// </summary>
		/// <returns></returns>
		private string GetStreamName()
		{
			Stream baseStream = _fileWriter.BaseStream;
			FileStream fileStream = (FileStream) baseStream;
			return fileStream.Name;
		}

		public void Dispose()
		{
			_fileWriter?.Dispose();
		}
	}
}