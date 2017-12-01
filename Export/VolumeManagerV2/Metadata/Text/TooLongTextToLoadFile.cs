using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class TooLongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly FromFileToLoadFileWriter _fileWriter;

		public TooLongTextToLoadFile(LongTextHelper longTextHelper, FromFileToLoadFileWriter fileWriter)
		{
			_longTextHelper = longTextHelper;
			_fileWriter = fileWriter;
		}

		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			ViewFieldInfo fieldToGetValueFrom;
			//TODO yyy? maybe field.AvfColumnName == TextPrecedence ? or maybe if we have field named TextPrecedence in Relativity then it will fail?
			if (field is CoalescedTextViewField)
			{
				fieldToGetValueFrom = _longTextHelper.GetTextPrecedenceTrueField(artifact, field);
			}
			else
			{
				fieldToGetValueFrom = field;
			}
			string longTextFileLocation = _longTextHelper.GetLongTextFileLocation(artifact, fieldToGetValueFrom);
			Encoding longTextFieldFileEncoding = _longTextHelper.GetLongTextFieldFileEncoding(fieldToGetValueFrom);
			lineEntry.AddPartialEntry(new LongTextWriteDeferredEntry(longTextFileLocation, longTextFieldFileEncoding, _fileWriter));
		}
	}
}