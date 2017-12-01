using System;
using System.IO;
using System.Threading;
using kCura.WinEDDS.Exceptions;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public abstract class MetadataFileWriter : IDisposable
	{
		private long _fileWriterLastPosition;

		private readonly StatisticsWrapper _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly RetryPolicy _retryPolicy;
		private readonly IDestinationPath _destinationPath;
		private readonly StreamFactory _streamFactory;

		protected StreamWriter _fileWriter;
		protected ILog Logger { get; }

		/// <summary>
		///     TODO refactor this class. It has too many dependencies
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="statistics"></param>
		/// <param name="fileHelper"></param>
		/// <param name="retryPolicy"></param>
		/// <param name="destinationPath"></param>
		/// <param name="streamFactory"></param>
		protected MetadataFileWriter(ILog logger, StatisticsWrapper statistics, IFileHelper fileHelper, RetryPolicy retryPolicy, IDestinationPath destinationPath,
			StreamFactory streamFactory)
		{
			Logger = logger;
			_statistics = statistics;
			_fileHelper = fileHelper;
			_retryPolicy = retryPolicy;
			_destinationPath = destinationPath;
			_streamFactory = streamFactory;

			_fileWriterLastPosition = 0;

			//TODO remove this
			_fileWriter = _streamFactory.Create(_fileWriter, _fileWriterLastPosition, _destinationPath.Path, _destinationPath.Encoding, false);
		}

		protected void ExecuteWithRetry(Action<Context, CancellationToken> action, CancellationToken token)
		{
			_retryPolicy.Execute(action, new Context(GetType().ToString()), token);
		}

		protected void ReinitializeStream()
		{
			//TODO this will open stream for every batch - the question is: is this impacting performance in a way we should change it?
			_fileWriter = _streamFactory.Create(_fileWriter, _fileWriterLastPosition, _destinationPath.Path, _destinationPath.Encoding, true);
		}

		protected void FlushStream()
		{
			try
			{
				_fileWriter?.Flush();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to flush {type} file stream.", GetLoadFileContext());
				throw new FileWriteException(GetLoadFileContext(), ex);
			}
		}

		protected abstract FileWriteException.DestinationFile GetLoadFileContext();

		protected void SaveStreamPositionAndUpdateStatistics()
		{
			if (_fileWriter != null)
			{
				_fileWriterLastPosition = _fileWriter.BaseStream.Position;
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