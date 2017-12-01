using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ImageLoadFileWriterFactory
	{
		private readonly ILog _logger;
		private readonly StatisticsWrapper _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly ImageLoadFileDestinationPath _destinationPath;
		private readonly StreamFactory _streamFactory;
		private readonly WritersRetryPolicy _writersRetryPolicy;

		public ImageLoadFileWriterFactory(ILog logger, StatisticsWrapper statistics, IFileHelper fileHelper, ImageLoadFileDestinationPath destinationPath, StreamFactory streamFactory,
			WritersRetryPolicy writersRetryPolicy)
		{
			_logger = logger;
			_statistics = statistics;
			_fileHelper = fileHelper;
			_destinationPath = destinationPath;
			_streamFactory = streamFactory;
			_writersRetryPolicy = writersRetryPolicy;
		}

		public ImageLoadFileWriter Create()
		{
			RetryPolicy retryPolicy = _writersRetryPolicy.CreateRetryPolicyForImageLoadFile();
			return new ImageLoadFileWriter(_logger, _statistics, _fileHelper, retryPolicy, _destinationPath, _streamFactory);
		}
	}
}