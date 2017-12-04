using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextHandlerFactory
	{
		private readonly IDelimiter _delimiter;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly LongTextRepository _longTextRepository;
		private readonly LongTextToLoadFile _longTextToLoadFile;
		private readonly LongTextHelper _longTextHelper;
		private readonly ILog _logger;

		public LongTextHandlerFactory(IDelimiter delimiter, IFilePathTransformer filePathTransformer, LongTextRepository longTextRepository, LongTextToLoadFile longTextToLoadFile,
			LongTextHelper longTextHelper, ILog logger)
		{
			_delimiter = delimiter;
			_filePathTransformer = filePathTransformer;
			_longTextRepository = longTextRepository;
			_longTextToLoadFile = longTextToLoadFile;
			_longTextHelper = longTextHelper;
			_logger = logger;
		}

		public ILongTextHandler Create(ExportFile exportFile)
		{
			ILongTextHandler textPrecedenceHandler;
			if (exportFile.ExportFullTextAsFile)
			{
				_logger.LogVerbose("Exporting full text as file - creating {type}.", nameof(LongTextToFile));
				textPrecedenceHandler = new LongTextToFile(exportFile, _filePathTransformer, _longTextRepository, _longTextHelper);
			}
			else
			{
				_logger.LogVerbose("Storing full text in load file - creating {type}.", nameof(LongTextToLoadFile));
				textPrecedenceHandler = _longTextToLoadFile;
			}
			return new LongTextHandler(textPrecedenceHandler, _longTextToLoadFile, _delimiter, _logger);
		}
	}
}