namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

	using Relativity.Logging;

	public class LongTextEncodingConverterFactory : ILongTextEncodingConverterFactory
	{
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileEncodingConverter _fileEncodingConverter;

		private readonly IErrorFileWriter _errorFileWriter;

		private readonly ILog _logger;

		public LongTextEncodingConverterFactory(LongTextRepository longTextRepository,
												IFileEncodingConverter fileEncodingConverter,
												IErrorFileWriter errorFileWriter,
												ILog logger)
		{
			_longTextRepository = longTextRepository;
			_fileEncodingConverter = fileEncodingConverter;
			_errorFileWriter = errorFileWriter;
			_logger = logger;
		}

		public IFileDownloadSubscriber Create(CancellationToken cancellationToken)
		{
			return new LongTextEncodingConverter2(_longTextRepository, _fileEncodingConverter, _errorFileWriter, _logger, cancellationToken);
		}
	}
}