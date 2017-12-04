using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextRepositoryBuilderFactory
	{
		private readonly ExportFile _exportSettings;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFieldService _fieldService;
		private readonly LabelManager _labelManager;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly IDirectoryHelper _directoryHelper;

		public LongTextRepositoryBuilderFactory(ExportFile exportSettings, LongTextHelper longTextHelper, IFieldService fieldService, LabelManager labelManager,
			IFileNameProvider fileNameProvider, IDirectoryHelper directoryHelper)
		{
			_exportSettings = exportSettings;
			_longTextHelper = longTextHelper;
			_fieldService = fieldService;
			_labelManager = labelManager;
			_fileNameProvider = fileNameProvider;
			_directoryHelper = directoryHelper;
		}

		public LongTextRepositoryBuilder Create(ExportFile exportFile)
		{
			ILongTextBuilder longTextPrecedenceBuilder = GetLongTextPrecedenceBuilder();
			ILongTextBuilder longTextFieldBuilder = new LongTextFieldBuilder(_fieldService, _longTextHelper);
			ILongTextBuilder longTextIproFullTextBuilder = GetLongTextIproFullTextBuilder();
			return new LongTextRepositoryBuilder(_longTextHelper, longTextPrecedenceBuilder, longTextFieldBuilder, longTextIproFullTextBuilder);
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
				longTextPrecedenceBuilder = new LongTextPrecedenceBuilder(_exportSettings, _fieldService, _longTextHelper, _labelManager, _fileNameProvider, _directoryHelper);
			}
			else
			{
				longTextPrecedenceBuilder = new EmptyLongTextBuilder();
			}
			return longTextPrecedenceBuilder;
		}
	}
}