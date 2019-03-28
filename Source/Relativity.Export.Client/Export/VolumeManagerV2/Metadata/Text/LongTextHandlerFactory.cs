using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextHandlerFactory
	{
		private readonly IDelimiter _delimiter;
		private readonly ILog _logger;

		public LongTextHandlerFactory(IDelimiter delimiter, ILog logger)
		{
			_delimiter = delimiter;
			_logger = logger;
		}

		public ILongTextHandler Create(ExportFile exportFile, IWindsorContainer container)
		{
			ILongTextHandler textPrecedenceHandler;
			if (exportFile.ExportFullTextAsFile)
			{
				_logger.LogVerbose("Exporting full text as file - creating {type}.", nameof(LongTextToFile));
				textPrecedenceHandler = container.Resolve<LongTextToFile>();
			}
			else
			{
				_logger.LogVerbose("Storing full text in load file - creating {type}.", nameof(LongTextToLoadFile));
				textPrecedenceHandler = container.Resolve<LongTextToLoadFile>();
			}

			LongTextToLoadFile longTextToLoadFile = container.Resolve<LongTextToLoadFile>();
			return new LongTextHandler(textPrecedenceHandler, longTextToLoadFile, _delimiter, _logger);
		}
	}
}