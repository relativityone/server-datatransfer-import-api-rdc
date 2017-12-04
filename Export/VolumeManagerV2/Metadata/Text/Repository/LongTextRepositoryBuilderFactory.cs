using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

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

		public LongTextRepositoryBuilderFactory(ExportFile exportSettings, LongTextFilePathProvider filePathProvider, LongTextHelper longTextHelper, IFieldService fieldService,
			IFileNameProvider fileNameProvider, LongTextRepository longTextRepository)
		{
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_longTextHelper = longTextHelper;
			_fieldService = fieldService;
			_fileNameProvider = fileNameProvider;
			_longTextRepository = longTextRepository;
		}

		public LongTextRepositoryBuilder Create(ExportFile exportFile)
		{
			ILongTextBuilder longTextPrecedenceBuilder = GetLongTextPrecedenceBuilder();
			ILongTextBuilder longTextFieldBuilder = new LongTextFieldBuilder(_fieldService, _longTextHelper);
			ILongTextBuilder longTextIproFullTextBuilder = GetLongTextIproFullTextBuilder();
			return new LongTextRepositoryBuilder(_longTextHelper, longTextPrecedenceBuilder, longTextFieldBuilder, longTextIproFullTextBuilder, _longTextRepository);
		}

		private ILongTextBuilder GetLongTextIproFullTextBuilder()
		{
			ILongTextBuilder longTextIproFullTextBuilder;
			if (!_longTextHelper.IsTextPrecedenceSet() && _exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText && _longTextHelper.IsExtractedTextMissing())
			{
				longTextIproFullTextBuilder = new LongTextIproFullTextBuilder(_longTextHelper);
			}
			else
			{
				longTextIproFullTextBuilder = new EmptyLongTextBuilder();
			}
			return longTextIproFullTextBuilder;
		}

		private ILongTextBuilder GetLongTextPrecedenceBuilder()
		{
			ILongTextBuilder longTextPrecedenceBuilder;
			if (_longTextHelper.IsTextPrecedenceSet())
			{
				longTextPrecedenceBuilder = new LongTextPrecedenceBuilder(_exportSettings, _filePathProvider, _fieldService, _longTextHelper, _fileNameProvider);
			}
			else
			{
				longTextPrecedenceBuilder = new EmptyLongTextBuilder();
			}
			return longTextPrecedenceBuilder;
		}
	}
}