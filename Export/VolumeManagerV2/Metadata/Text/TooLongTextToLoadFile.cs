using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class TooLongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly LongTextRepository _longTextRepository;
		private readonly FromFileToLoadFileWriter _fileWriter;
		private readonly ILog _logger;

		public TooLongTextToLoadFile(LongTextHelper longTextHelper, LongTextRepository longTextRepository, FromFileToLoadFileWriter fileWriter, ILog logger)
		{
			_longTextHelper = longTextHelper;
			_longTextRepository = longTextRepository;
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
			LongText longText = _longTextRepository.GetLongText(artifact.ArtifactID, fieldToGetValueFrom.FieldArtifactId);
			lineEntry.AddPartialEntry(new LongTextWriteDeferredEntry(longTextFileLocation, longText.DestinationEncoding, _fileWriter));
		}
	}
}