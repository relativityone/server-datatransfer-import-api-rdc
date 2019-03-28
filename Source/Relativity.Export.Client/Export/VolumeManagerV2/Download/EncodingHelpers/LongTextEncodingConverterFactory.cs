using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers
{
	public class LongTextEncodingConverterFactory
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

		public LongTextEncodingConverter Create(CancellationToken cancellationToken)
		{
			return new LongTextEncodingConverter(_longTextRepository, _fileEncodingConverter, _logger, cancellationToken);
		}
	}
}