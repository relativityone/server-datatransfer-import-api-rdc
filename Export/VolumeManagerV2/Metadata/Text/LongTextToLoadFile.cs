using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly TooLongTextToLoadFile _tooLongTextToLoadFile;
		private readonly NotTooLongTextToLoadFile _notTooLongTextToLoadFile;

		public LongTextToLoadFile(LongTextHelper longTextHelper, TooLongTextToLoadFile tooLongTextToLoadFile, NotTooLongTextToLoadFile notTooLongTextToLoadFile)
		{
			_longTextHelper = longTextHelper;
			_tooLongTextToLoadFile = tooLongTextToLoadFile;
			_notTooLongTextToLoadFile = notTooLongTextToLoadFile;
		}


		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			if (_longTextHelper.IsTextTooLong(artifact, field.AvfColumnName))
			{
				_tooLongTextToLoadFile.HandleLongText(artifact, field, lineEntry);
			}
			else
			{
				_notTooLongTextToLoadFile.HandleLongText(artifact, field, lineEntry);
			}
		}
	}
}