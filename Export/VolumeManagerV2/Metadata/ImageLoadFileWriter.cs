using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.Exporters;
using Polly;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public class ImageLoadFileWriter : IDisposable
	{
		private long _imageFileWriterLastPosition;

		private StreamWriter _imageFileWriter;
		private readonly ImageLoadFileDestinationPath _imageLoadFileDestinationPath;
		private readonly StreamFactory _streamFactory;
		private readonly StatisticsWrapper _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly RetryPolicy _retryPolicy;
		private readonly ILog _logger;

		/// <summary>
		/// TODO refactor this class. It has to many dependencies
		/// </summary>
		/// <param name="statistics"></param>
		/// <param name="fileHelper"></param>
		/// <param name="retryPolicy"></param>
		/// <param name="streamFactory"></param>
		/// <param name="imageLoadFileDestinationPath"></param>
		/// <param name="logger"></param>
		public ImageLoadFileWriter(StatisticsWrapper statistics, IFileHelper fileHelper, RetryPolicy retryPolicy, StreamFactory streamFactory,
			ImageLoadFileDestinationPath imageLoadFileDestinationPath, ILog logger)
		{
			_statistics = statistics;
			_fileHelper = fileHelper;
			_retryPolicy = retryPolicy;
			_streamFactory = streamFactory;
			_imageLoadFileDestinationPath = imageLoadFileDestinationPath;
			_logger = logger;

			_imageFileWriterLastPosition = 0;
		}

		public void Write(ConcurrentBag<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			if (linesToWrite == null || linesToWrite.Count == 0)
			{
				_logger.LogVerbose("No lines to write to image load file - skipping.");
				return;
			}

			_logger.LogVerbose("Writing to image load file with retry policy.");
			_retryPolicy.Execute((context, token) =>
			{
				Write(linesToWrite, artifacts, context);
			}, new Context(nameof(ImageLoadFileWriter)), cancellationToken);
		}

		private void Write(ConcurrentBag<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			//TODO this will open stream for every batch - the question is: is this impacting performance in a way we should change it?
			_imageFileWriter = _streamFactory.Create(_imageFileWriter, _imageFileWriterLastPosition, _imageLoadFileDestinationPath.Path, _imageLoadFileDestinationPath.Encoding);

			WriteArtifacts(linesToWrite, artifacts, context);

			FlushStream();

			SaveStreamPositionAndUpdateStatistics();
		}

		private void WriteArtifacts(ConcurrentBag<KeyValuePair<string, string>> linesToWrite, ObjectExportInfo[] artifacts, Context context)
		{
			//TODO this "sorting" was introduced after changing ConcurrentDictionary to ConcurrentBag - is it needed?
			foreach (var artifact in artifacts)
			{
				//TODO I don't like this :(
				context[WritersRetryPolicy.CONTEXT_LAST_ARTIFACT_ID_KEY] = artifact.ArtifactID;

				IEnumerable<ImageExportInfo> imagesList = artifact.Images.Cast<ImageExportInfo>();
				IEnumerable<string> bates = imagesList.Select(x => x.BatesNumber).Distinct();

				foreach (var bate in bates)
				{
					string key = bate;

					foreach (var line in linesToWrite.Where(x => x.Key == $"FF{key}").OrderBy(x => x.Key).ThenBy(x => x.Value))
					{
						_imageFileWriter.Write(line.Value);
					}

					foreach (var line in linesToWrite.Where(x => x.Key == key).OrderBy(x => x.Key).ThenBy(x => x.Value))
					{
						_imageFileWriter.Write(line.Value);
					}
				}
			}
		}

		private void FlushStream()
		{
			try
			{
				_imageFileWriter?.Flush();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to flush image file stream.");
				throw new FileWriteException(FileWriteException.DestinationFile.Image, ex);
			}
		}

		private void SaveStreamPositionAndUpdateStatistics()
		{
			if (_imageFileWriter != null)
			{
				_imageFileWriterLastPosition = _imageFileWriter.BaseStream.Position;
				_statistics.MetadataBytes = _fileHelper.GetFileSize(GetStreamName());
			}
		}

		/// <summary>
		///     TODO it should be the same as _imageLoadFileDestinationPath.Path, right?
		/// </summary>
		/// <returns></returns>
		private string GetStreamName()
		{
			Stream baseStream = _imageFileWriter.BaseStream;
			FileStream fileStream = (FileStream) baseStream;
			return fileStream.Name;
		}

		public void Dispose()
		{
			_imageFileWriter?.Dispose();
		}
	}
}