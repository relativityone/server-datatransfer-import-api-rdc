using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
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
		private readonly LongTextHelper _longTextHelper;

		public TextExportRequestBuilder(ExportFile exportSettings, IFieldService fieldService, DownloadedTextFilesRepository textFilesRepository, LongTextHelper longTextHelper)
		{
			_exportSettings = exportSettings;
			_fieldService = fieldService;
			_textFilesRepository = textFilesRepository;
			_longTextHelper = longTextHelper;
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
					if (_longTextHelper.IsTextTooLong(artifact, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME))
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
			for (int i = 0; i < _fieldService.GetColumns().Length; i++)
			{
				ViewFieldInfo fieldInfo = _fieldService.GetColumns()[i];
				if (_longTextHelper.IsLongTextField(fieldInfo))
				{
					CreateRequest(artifact, fieldInfo);
				}
			}
		}

		private void CreateRequestForIproFullTextWithoutPrecedence(ObjectExportInfo artifact)
		{
			if (_longTextHelper.IsTextPrecedenceSet() || _exportSettings.LogFileFormat != LoadFileType.FileFormat.IPRO_FullText)
			{
				return;
			}

			if (_longTextHelper.IsExtractedTextMissing())
			{
				if (_longTextHelper.IsTextTooLong(artifact, LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME))
				{
					string tempLocation = Path.GetTempFileName();
					int extractedTextFieldId = _longTextHelper.GetFieldArtifactId(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
					AddTextExportRequest(TextExportRequest.CreateRequestForFullText(artifact, extractedTextFieldId, tempLocation));
				}
			}
		}

		private void CreateRequest(ObjectExportInfo artifact, ViewFieldInfo fieldInfo)
		{
			if (_longTextHelper.IsTextTooLong(artifact, fieldInfo.AvfColumnName))
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
			_textFilesRepository.AddTextExportLocation(textExportRequest);
			_exportRequests.Add(textExportRequest);
		}
	}
}