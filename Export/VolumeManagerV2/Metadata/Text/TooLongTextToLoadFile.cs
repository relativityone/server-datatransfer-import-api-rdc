using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class TooLongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly FromFileToLoadFileWriter _fileWriter;
		private readonly ILog _logger;

		public TooLongTextToLoadFile(LongTextHelper longTextHelper, FromFileToLoadFileWriter fileWriter, ILog logger)
		{
			_longTextHelper = longTextHelper;
			_fileWriter = fileWriter;
			_logger = logger;
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
			_logger.LogVerbose("Passing LongText file location to writer for future processing. Field {fieldName} (field used for Text Precedence {name}.", field.AvfColumnName,
				fieldToGetValueFrom.AvfColumnName);
			string longTextFileLocation = _longTextHelper.GetLongTextFileLocation(artifact, fieldToGetValueFrom);
			Encoding longTextFieldFileEncoding = _longTextHelper.GetLongTextFieldFileEncoding(fieldToGetValueFrom);
			lineEntry.AddPartialEntry(new LongTextWriteDeferredEntry(longTextFileLocation, longTextFieldFileEncoding, _fileWriter));
		}
	}
}