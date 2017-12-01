using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class FullTextLoadFileEntryFactory
	{
		private readonly IFieldService _fieldService;
		private readonly LongTextHelper _longTextHelper;

		public FullTextLoadFileEntryFactory(IFieldService fieldService, LongTextHelper longTextHelper)
		{
			_fieldService = fieldService;
			_longTextHelper = longTextHelper;
		}

		public IFullTextLoadFileEntry Create(ExportFile exportSettings)
		{
			//TODO validate arguments
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
			{
				if (_longTextHelper.IsTextPrecedenceSet())
				{
					return new IproFullTextWithPrecedenceLoadFileEntry(exportSettings, _fieldService, _longTextHelper);
				}
				return new IproFullTextWithoutPrecedenceLoadFileEntry(exportSettings, _fieldService, _longTextHelper);
			}
			return new NoFullTextLoadFileEntry();
		}
	}
}