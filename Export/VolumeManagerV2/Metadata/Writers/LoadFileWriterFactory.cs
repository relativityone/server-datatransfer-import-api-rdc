using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using Polly.Retry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class LoadFileWriterFactory
	{
		private readonly ILog _logger;
		private readonly StatisticsWrapper _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly LoadFileDestinationPath _destinationPath;
		private readonly StreamFactory _streamFactory;
		private readonly WritersRetryPolicy _writersRetryPolicy;

		public LoadFileWriterFactory(ILog logger, StatisticsWrapper statistics, IFileHelper fileHelper, LoadFileDestinationPath destinationPath, StreamFactory streamFactory,
			WritersRetryPolicy writersRetryPolicy)
		{
			_logger = logger;
			_statistics = statistics;
			_fileHelper = fileHelper;
			_destinationPath = destinationPath;
			_streamFactory = streamFactory;
			_writersRetryPolicy = writersRetryPolicy;
		}

		public LoadFileWriter Create()
		{
			RetryPolicy retryPolicy = _writersRetryPolicy.CreateRetryPolicyForLoadFile();
			return new LoadFileWriter(_logger, _statistics, _fileHelper, retryPolicy, _destinationPath, _streamFactory);
		}
	}
}