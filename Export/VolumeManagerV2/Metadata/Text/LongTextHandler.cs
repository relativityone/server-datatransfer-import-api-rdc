using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextHandler : ILongTextHandler
	{
		private readonly ILongTextHandler _textPrecedenceHandler;
		private readonly LongTextToLoadFile _textToLoadFile;
		private readonly IDelimiter _delimiter;

		public LongTextHandler(ILongTextHandler textPrecedenceHandler, LongTextToLoadFile textToLoadFile, IDelimiter delimiter)
		{
			_textPrecedenceHandler = textPrecedenceHandler;
			_textToLoadFile = textToLoadFile;
			_delimiter = delimiter;
		}

		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			lineEntry.AddStringEntry(_delimiter.Start);

			if (field is CoalescedTextViewField)
			{
				_textPrecedenceHandler.HandleLongText(artifact, field, lineEntry);
			}
			else
			{
				_textToLoadFile.HandleLongText(artifact, field, lineEntry);
			}

			lineEntry.AddStringEntry(_delimiter.End);
		}
	}
}