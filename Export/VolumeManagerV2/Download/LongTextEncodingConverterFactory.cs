using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class LongTextEncodingConverterFactory
	{
		private readonly ExportFile _exportSettings;
		private readonly LongTextRepository _longTextRepository;
		private readonly FileEncodingConverter _fileEncodingConverter;
		private readonly ILog _logger;

		public LongTextEncodingConverterFactory(ExportFile exportSettings, LongTextRepository longTextRepository, FileEncodingConverter fileEncodingConverter, ILog logger)
		{
			_exportSettings = exportSettings;
			_longTextRepository = longTextRepository;
			_fileEncodingConverter = fileEncodingConverter;
			_logger = logger;
		}

		public LongTextEncodingConverter Create(CancellationToken cancellationToken)
		{
			return new LongTextEncodingConverter(_exportSettings, _longTextRepository, _fileEncodingConverter, _logger, cancellationToken);
		}
	}
}