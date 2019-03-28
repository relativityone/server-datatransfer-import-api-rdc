using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class LongTextRepositoryBuilderFactory
	{
		private readonly ExportFile _exportSettings;
		private readonly LongTextHelper _longTextHelper;
		private readonly LongTextRepository _longTextRepository;
		private readonly ILog _logger;

		public LongTextRepositoryBuilderFactory(ExportFile exportSettings, LongTextHelper longTextHelper, LongTextRepository longTextRepository, ILog logger)
		{
			_exportSettings = exportSettings;
			_longTextHelper = longTextHelper;
			_longTextRepository = longTextRepository;
			_logger = logger;
		}

		public LongTextRepositoryBuilder Create(ExportFile exportFile, IWindsorContainer container)
		{
			ILongTextBuilder longTextPrecedenceBuilder = GetLongTextPrecedenceBuilder(container);
			ILongTextBuilder longTextFieldBuilder = container.Resolve<LongTextFromFieldBuilder>();
			ILongTextBuilder longTextIproFullTextBuilder = GetLongTextIproFullTextBuilder(container);
			return new LongTextRepositoryBuilder(longTextPrecedenceBuilder, longTextFieldBuilder, longTextIproFullTextBuilder, _longTextRepository, _logger);
		}

		private ILongTextBuilder GetLongTextIproFullTextBuilder(IWindsorContainer container)
		{
			ILongTextBuilder longTextIproFullTextBuilder;
			if (!_longTextHelper.IsTextPrecedenceSet() && _exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText && _longTextHelper.IsExtractedTextMissing())
			{
				_logger.LogVerbose("Text precedence is not set, IPRO Full Text format selected, ExtractedText column is not mapped - creating {type}.", nameof(LongTextIproFullTextBuilder));
				longTextIproFullTextBuilder = container.Resolve<LongTextIproFullTextBuilder>();
			}
			else
			{
				_logger.LogVerbose("Text precedence is set, IPRO Full Text format is not selected selected or ExtractedText column is mapped - creating {type}.", nameof(EmptyLongTextBuilder));
				longTextIproFullTextBuilder = container.Resolve<EmptyLongTextBuilder>();
			}

			return longTextIproFullTextBuilder;
		}

		private ILongTextBuilder GetLongTextPrecedenceBuilder(IWindsorContainer container)
		{
			ILongTextBuilder longTextPrecedenceBuilder;
			if (_longTextHelper.IsTextPrecedenceSet())
			{
				_logger.LogVerbose("Text precedence is set - creating {type}.", nameof(LongTextPrecedenceBuilder));
				longTextPrecedenceBuilder = container.Resolve<LongTextPrecedenceBuilder>();
			}
			else
			{
				_logger.LogVerbose("Text precedence is not set - creating {type}.", nameof(EmptyLongTextBuilder));
				longTextPrecedenceBuilder = container.Resolve<EmptyLongTextBuilder>();
			}

			return longTextPrecedenceBuilder;
		}
	}
}