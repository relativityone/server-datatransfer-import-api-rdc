namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text
{
	using Castle.Core;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.Logging;

	public class TooLongTextToLoadFile : ILongTextHandler
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly LongTextRepository _longTextRepository;
		private readonly ILongTextEntryWriter _fileWriter;
		private readonly ILog _logger;

		public TooLongTextToLoadFile(LongTextHelper longTextHelper, LongTextRepository longTextRepository, FromFileToLoadFileWriter fileWriter, ILog logger)
			: this(longTextHelper, longTextRepository, (ILongTextEntryWriter) fileWriter, logger)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="longTextHelper"></param>
		/// <param name="longTextRepository"></param>
		/// <param name="fileWriter"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public TooLongTextToLoadFile(LongTextHelper longTextHelper, LongTextRepository longTextRepository, ILongTextEntryWriter fileWriter, ILog logger)
		{
			_longTextHelper = longTextHelper;
			_longTextRepository = longTextRepository;
			_fileWriter = fileWriter;
			_logger = logger;
		}

		public void HandleLongText(ObjectExportInfo artifact, kCura.WinEDDS.ViewFieldInfo field, DeferredEntry lineEntry)
		{
			kCura.WinEDDS.ViewFieldInfo fieldToGetValueFrom;
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
			string longTextFileLocation = _longTextHelper.GetLongTextFileLocation(artifact, fieldToGetValueFrom.FieldArtifactId);
			LongText longText = _longTextRepository.GetLongText(artifact.ArtifactID, fieldToGetValueFrom.FieldArtifactId);
			lineEntry.AddPartialEntry(new LongTextWriteDeferredEntry(longTextFileLocation, longText.DestinationEncoding, _fileWriter));
		}
	}
}