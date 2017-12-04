using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextRepositoryBuilderFactory
	{
		private readonly ExportFile _exportSettings;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFieldService _fieldService;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly LongTextFilePathProvider _filePathProvider;
		private readonly LongTextRepository _longTextRepository;
		private readonly ILog _logger;

		public LongTextRepositoryBuilderFactory(ExportFile exportSettings, LongTextFilePathProvider filePathProvider, LongTextHelper longTextHelper, IFieldService fieldService,
			IFileNameProvider fileNameProvider, LongTextRepository longTextRepository, ILog logger)
		{
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_longTextHelper = longTextHelper;
			_fieldService = fieldService;
			_fileNameProvider = fileNameProvider;
			_longTextRepository = longTextRepository;
			_logger = logger;
		}

		public LongTextRepositoryBuilder Create(ExportFile exportFile)
		{
			ILongTextBuilder longTextPrecedenceBuilder = GetLongTextPrecedenceBuilder();
			ILongTextBuilder longTextFieldBuilder = new LongTextFromFieldBuilder(_fieldService, _longTextHelper, _logger);
			ILongTextBuilder longTextIproFullTextBuilder = GetLongTextIproFullTextBuilder();
			return new LongTextRepositoryBuilder(longTextPrecedenceBuilder, longTextFieldBuilder, longTextIproFullTextBuilder, _longTextRepository, _logger);
		}

		private ILongTextBuilder GetLongTextIproFullTextBuilder()
		{
			ILongTextBuilder longTextIproFullTextBuilder;
			if (!_longTextHelper.IsTextPrecedenceSet() && _exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText && _longTextHelper.IsExtractedTextMissing())
			{
				_logger.LogVerbose("Text precedence is not set, IPRO Full Text format selected, ExtractedText column is not mapped - creating {type}.", nameof(LongTextIproFullTextBuilder));
				longTextIproFullTextBuilder = new LongTextIproFullTextBuilder(_longTextHelper, _logger);
			}
			else
			{
				_logger.LogVerbose("Text precedence is set, IPRO Full Text format is not selected selected or ExtractedText column is mapped - creating {type}.", nameof(EmptyLongTextBuilder));
				longTextIproFullTextBuilder = new EmptyLongTextBuilder();
			}
			return longTextIproFullTextBuilder;
		}

		private ILongTextBuilder GetLongTextPrecedenceBuilder()
		{
			ILongTextBuilder longTextPrecedenceBuilder;
			if (_longTextHelper.IsTextPrecedenceSet())
			{
				_logger.LogVerbose("Text precedence is set - creating {type}.", nameof(LongTextPrecedenceBuilder));
				longTextPrecedenceBuilder = new LongTextPrecedenceBuilder(_exportSettings, _filePathProvider, _fieldService, _longTextHelper, _fileNameProvider, _logger);
			}
			else
			{
				_logger.LogVerbose("Text precedence is not set - creating {type}.", nameof(EmptyLongTextBuilder));
				longTextPrecedenceBuilder = new EmptyLongTextBuilder();
			}
			return longTextPrecedenceBuilder;
		}
	}
}