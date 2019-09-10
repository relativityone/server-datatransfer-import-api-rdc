namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

	using Relativity.Logging;

	public class LongTextEncodingConverterFactory : ILongTextEncodingConverterFactory
	{
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileEncodingConverter _fileEncodingConverter;
		private readonly ILog _logger;

		public LongTextEncodingConverterFactory(LongTextRepository longTextRepository, IFileEncodingConverter fileEncodingConverter, ILog logger)
		{
			_longTextRepository = longTextRepository;
			_fileEncodingConverter = fileEncodingConverter;
			_logger = logger;
		}

		public IFileDownloadSubscriber Create(CancellationToken cancellationToken)
		{
			return new LongTextEncodingConverter(_longTextRepository, _fileEncodingConverter, _logger, cancellationToken);
		}
	}
}