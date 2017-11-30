using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Exporters;
using Relativity;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class TextExportRequestBuilder
	{
		private IList<TextExportRequest> _exportRequests;

		private readonly ExportFile _exportSettings;
		private readonly IFieldService _fieldService;
		private readonly DownloadedTextFilesRepository _textFilesRepository;

		public TextExportRequestBuilder(ExportFile exportSettings, IFieldService fieldService, DownloadedTextFilesRepository textFilesRepository)
		{
			_exportSettings = exportSettings;
			_fieldService = fieldService;
			_textFilesRepository = textFilesRepository;
		}

		public IList<TextExportRequest> Create(ObjectExportInfo artifact)
		{
			_exportRequests = new List<TextExportRequest>();

			CreateRequestForPrecedence(artifact);

			CreateRequestForTextFields(artifact);

			CreateRequestForIproFullTextWithoutPrecedence(artifact);

			return _exportRequests;
		}

		private void CreateRequestForPrecedence(ObjectExportInfo artifact)
		{
			ViewFieldInfo field = GetFieldForLongTextPrecedenceDownload(artifact);
			if (field != null)
			{
				if (_fieldService.ContainsFieldName(Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME))
				{
					if (artifact.IsTextTooLong(_fieldService, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME))
					{
						string tempLocation = Path.GetTempFileName();

						if (_exportSettings.ArtifactTypeID == (int) ArtifactType.Document && field.Category == FieldCategory.FullText && !(field is CoalescedTextViewField))
						{
							AddTextExportRequest(TextExportRequest.CreateRequestForFullText(artifact, field.FieldArtifactId, tempLocation));
						}
						else
						{
							ViewFieldInfo fieldToExport = GetFieldForLongTextPrecedenceDownload(artifact, field);
							AddTextExportRequest(TextExportRequest.CreateRequestForLongText(artifact, fieldToExport.FieldArtifactId, tempLocation));
						}
					}
				}
			}
		}

		private void CreateRequestForTextFields(ObjectExportInfo artifact)
		{
			for (int i = 0; i < _fieldService.GetColumns().Count; i++)
			{
				var fieldInfo = (ViewFieldInfo) _fieldService.GetColumns()[i];
				if (fieldInfo.FieldType.IsTextField())
				{
					CreateRequest(artifact, fieldInfo);
				}
			}
		}

		private void CreateRequestForIproFullTextWithoutPrecedence(ObjectExportInfo artifact)
		{
			if (_exportSettings.IsTextPrecedenceSet() || _exportSettings.LogFileFormat != LoadFileType.FileFormat.IPRO_FullText)
			{
				return;
			}
			int fieldIndex = _fieldService.GetOrdinalIndex(FieldUtils.EXTRACTED_TEXT_COLUMN_NAME);
			
			if (fieldIndex >= _fieldService.GetColumns().Count)
			{
				//Missing ExtractedText field for IPRO_FullText
				if (artifact.IsTextTooLong(_fieldService, FieldUtils.EXTRACTED_TEXT_COLUMN_NAME))
				{
					string tempLocation = Path.GetTempFileName();
					AddTextExportRequest(TextExportRequest.CreateRequestForFullText(artifact, FieldUtils.EXTRACTED_TEXT_FIELD_ID, tempLocation));
				}
			}
		}

		private void CreateRequest(ObjectExportInfo artifact, ViewFieldInfo fieldInfo)
		{
			if (artifact.IsTextTooLong(_fieldService, fieldInfo.AvfColumnName))
			{
				if (!(fieldInfo is CoalescedTextViewField) && !_textFilesRepository.IsTextFileDownloaded(artifact.ArtifactID, fieldInfo.FieldArtifactId))
				{
					string tempLocation = Path.GetTempFileName();
					AddTextExportRequest(TextExportRequest.CreateRequestForLongText(artifact, fieldInfo.FieldArtifactId, tempLocation));
				}
			}
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

		private void AddTextExportRequest(TextExportRequest textExportRequest)
		{
			//TODO REMOVE THIS!!!! THIS IS JUST FOR TESTING BEFORE WE GET CLIENT FOR LONG TEXT
			File.WriteAllText(textExportRequest.DestinationLocation, $"Long text sample for Field {textExportRequest.FieldArtifactId} in Artifact {textExportRequest.ArtifactId}.");
			//********************************************************************************
			_textFilesRepository.AddTextExportLocation(textExportRequest);
			_exportRequests.Add(textExportRequest);
		}
	}
}