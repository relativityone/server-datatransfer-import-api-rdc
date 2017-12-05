using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextPrecedenceBuilder : ILongTextBuilder
	{
		private readonly ExportFile _exportSettings;
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFieldService _fieldService;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly ILog _logger;

		public LongTextPrecedenceBuilder(ExportFile exportSettings, LongTextFilePathProvider filePathProvider, IFieldService fieldService, LongTextHelper longTextHelper,
			IFileNameProvider fileNameProvider, ILog logger)
		{
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_fieldService = fieldService;
			_longTextHelper = longTextHelper;
			_fileNameProvider = fileNameProvider;
			_logger = logger;
		}

		public IEnumerable<LongText> CreateLongText(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Attempting to create LongText for TextPrecedence field.");
			ViewFieldInfo field = GetFieldForLongTextPrecedenceDownload(artifact);

			_logger.LogVerbose("Text Precedence is stored in field {fieldName}:{fieldId}.", field.AvfColumnName, field.FieldArtifactId);

			if (_longTextHelper.IsTextTooLong(artifact, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME))
			{
				yield return CreateForTooLongText(artifact, field);
			}
			yield return CreateForLongText(artifact, field);
		}

		private LongText CreateForTooLongText(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			string destinationLocation = GetDestinationLocation(artifact);
			LongTextExportRequest longTextExportRequest = CreateExportRequest(artifact, field, destinationLocation);
			if (_exportSettings.ExportFullTextAsFile)
			{
				_logger.LogVerbose("LongText file missing - creating ExportRequest to destination file.");
				return LongText.CreateFromMissingFile(artifact.ArtifactID, longTextExportRequest.FieldArtifactId, longTextExportRequest);
			}
			_logger.LogVerbose("LongText file missing - creating ExportRequest to temporary file.");
			return LongText.CreateFromMissingValue(artifact.ArtifactID, longTextExportRequest.FieldArtifactId, longTextExportRequest);
		}

		private LongText CreateForLongText(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			ViewFieldInfo fieldForPrecedence = GetFieldForLongTextPrecedenceDownload(artifact, field);
			string longTextValue = _longTextHelper.GetTextFromField(artifact, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME);

			if (_exportSettings.ExportFullTextAsFile)
			{
				string destinationLocation = GetDestinationLocation(artifact);
				_logger.LogVerbose("LongText value exists - writing it to destination file {location}.", destinationLocation);
				using (StreamWriter streamWriter = new StreamWriter(destinationLocation, false, _exportSettings.TextFileEncoding))
				{
					streamWriter.Write(longTextValue);
				}
				return LongText.CreateFromExistingFile(artifact.ArtifactID, fieldForPrecedence.FieldArtifactId, destinationLocation);
			}
			_logger.LogVerbose("LongText value exists - storing it into memory.");
			return LongText.CreateFromExistingValue(artifact.ArtifactID, fieldForPrecedence.FieldArtifactId, longTextValue);
		}

		private LongTextExportRequest CreateExportRequest(ObjectExportInfo artifact, ViewFieldInfo field, string destinationLocation)
		{
			if (_exportSettings.ArtifactTypeID == (int) ArtifactType.Document && field.Category == FieldCategory.FullText && !(field is CoalescedTextViewField))
			{
				return LongTextExportRequest.CreateRequestForFullText(artifact, field.FieldArtifactId, destinationLocation);
			}
			ViewFieldInfo fieldToExport = GetFieldForLongTextPrecedenceDownload(artifact, field);
			return LongTextExportRequest.CreateRequestForLongText(artifact, fieldToExport.FieldArtifactId, destinationLocation);
		}

		private ViewFieldInfo GetFieldForLongTextPrecedenceDownload(ObjectExportInfo artifact)
		{
			if (_exportSettings.SelectedTextFields != null)
			{
				int fieldArtifactId = (int) artifact.Metadata[_fieldService.GetOrdinalIndex(Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)];
				return _exportSettings.SelectedTextFields.FirstOrDefault(x => x.FieldArtifactId == fieldArtifactId);
			}
			return null;
		}

		private ViewFieldInfo GetFieldForLongTextPrecedenceDownload(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			if (field == null || field.AvfColumnName == Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)
			{
				return GetFieldForLongTextPrecedenceDownload(artifact);
			}
			return field;
		}

		private string GetDestinationLocation(ObjectExportInfo artifact)
		{
			if (_exportSettings.ExportFullTextAsFile)
			{
				string fileName = _fileNameProvider.GetTextName(artifact);
				return _filePathProvider.GetPathForFile(fileName);
			}
			return Path.GetTempFileName();
		}
	}
}