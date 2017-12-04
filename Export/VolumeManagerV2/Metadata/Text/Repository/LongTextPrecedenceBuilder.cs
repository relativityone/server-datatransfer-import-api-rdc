using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextPrecedenceBuilder : ILongTextBuilder
	{
		private readonly ExportFile _exportSettings;
		private readonly IFieldService _fieldService;
		private readonly LongTextHelper _longTextHelper;
		private readonly LabelManager _labelManager;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly IDirectoryHelper _directoryHelper;

		public LongTextPrecedenceBuilder(ExportFile exportSettings, IFieldService fieldService, LongTextHelper longTextHelper, LabelManager labelManager,
			IFileNameProvider fileNameProvider, IDirectoryHelper directoryHelper)
		{
			_exportSettings = exportSettings;
			_fieldService = fieldService;
			_longTextHelper = longTextHelper;
			_labelManager = labelManager;
			_fileNameProvider = fileNameProvider;
			_directoryHelper = directoryHelper;
		}

		public IList<LongText> CreateLongText(ObjectExportInfo artifact)
		{
			ViewFieldInfo field = GetFieldForLongTextPrecedenceDownload(artifact);

			if (_longTextHelper.IsTextTooLong(artifact, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME))
			{
				return new List<LongText>
				{
					CreateForTooLongText(artifact, field)
				};
			}
			return new List<LongText>
			{
				CreateForLongText(artifact, field)
			};
		}

		private LongText CreateForTooLongText(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			string destinationLocation = GetDestinationLocation(artifact);
			TextExportRequest textExportRequest = CreateExportRequest(artifact, field, destinationLocation);
			if (_exportSettings.ExportFullTextAsFile)
			{
				return LongText.CreateFromMissingFile(artifact.ArtifactID, textExportRequest.FieldArtifactId, textExportRequest);
			}
			return LongText.CreateFromMissingValue(artifact.ArtifactID, textExportRequest.FieldArtifactId, textExportRequest);
		}

		private LongText CreateForLongText(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			ViewFieldInfo fieldForPrecedence = GetFieldForLongTextPrecedenceDownload(artifact, field);
			string longTextValue = _longTextHelper.GetTextFromField(artifact, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME);

			if (_exportSettings.ExportFullTextAsFile)
			{
				string destinationLocation = GetDestinationLocation(artifact);
				//TODO encoding
				using (StreamWriter streamWriter = new StreamWriter(destinationLocation, false, _exportSettings.TextFileEncoding))
				{
					streamWriter.Write(longTextValue);
				}
				return LongText.CreateFromExistingFile(artifact.ArtifactID, fieldForPrecedence.FieldArtifactId, destinationLocation);
			}
			return LongText.CreateFromExistingValue(artifact.ArtifactID, fieldForPrecedence.FieldArtifactId, longTextValue);
		}

		private TextExportRequest CreateExportRequest(ObjectExportInfo artifact, ViewFieldInfo field, string destinationLocation)
		{
			if (_exportSettings.ArtifactTypeID == (int) ArtifactType.Document && field.Category == FieldCategory.FullText && !(field is CoalescedTextViewField))
			{
				return TextExportRequest.CreateRequestForFullText(artifact, field.FieldArtifactId, destinationLocation);
			}
			ViewFieldInfo fieldToExport = GetFieldForLongTextPrecedenceDownload(artifact, field);
			return TextExportRequest.CreateRequestForLongText(artifact, fieldToExport.FieldArtifactId, destinationLocation);
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
			string destinationLocation;
			if (_exportSettings.ExportFullTextAsFile)
			{
				string volumeLabel = _labelManager.GetCurrentVolumeLabel();
				string subdirectoryLabel = _labelManager.GetCurrentTextSubdirectoryLabel();

				string destinationDirectory = Path.Combine(_exportSettings.FolderPath, volumeLabel, subdirectoryLabel);

				if (!_directoryHelper.Exists(destinationDirectory))
				{
					_directoryHelper.CreateDirectory(destinationDirectory);
				}

				destinationLocation = Path.Combine(destinationDirectory, _fileNameProvider.GetTextName(artifact));
			}
			else
			{
				destinationLocation = Path.GetTempFileName();
			}
			return destinationLocation;
		}
	}
}