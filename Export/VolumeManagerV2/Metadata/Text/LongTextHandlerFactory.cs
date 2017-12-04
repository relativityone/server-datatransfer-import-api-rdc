using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextHandlerFactory
	{
		private readonly IDelimiter _delimiter;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly LongTextRepository _longTextRepository;
		private readonly LongTextToLoadFile _longTextToLoadFile;
		private readonly LongTextHelper _longTextHelper;

		public LongTextHandlerFactory(IDelimiter delimiter, IFilePathTransformer filePathTransformer, LongTextRepository longTextRepository, LongTextToLoadFile longTextToLoadFile,
			LongTextHelper longTextHelper)
		{
			_delimiter = delimiter;
			_filePathTransformer = filePathTransformer;
			_longTextRepository = longTextRepository;
			_longTextToLoadFile = longTextToLoadFile;
			_longTextHelper = longTextHelper;
		}

		public ILongTextHandler Create(ExportFile exportFile)
		{
			ILongTextHandler textPrecedenceHandler;
			if (exportFile.ExportFullTextAsFile)
			{
				textPrecedenceHandler = new LongTextToFile(exportFile, _filePathTransformer, _longTextRepository, _longTextHelper);
			}
			else
			{
				textPrecedenceHandler = _longTextToLoadFile;
			}
			return new LongTextHandler(textPrecedenceHandler, _longTextToLoadFile, _delimiter);
		}
	}
}