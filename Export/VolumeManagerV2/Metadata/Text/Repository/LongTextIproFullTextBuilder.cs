using System.Collections.Generic;
using System.IO;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextIproFullTextBuilder : ILongTextBuilder
	{
		private readonly LongTextHelper _longTextHelper;

		public LongTextIproFullTextBuilder(LongTextHelper longTextHelper)
		{
			_longTextHelper = longTextHelper;
		}

		public IList<LongText> CreateLongText(ObjectExportInfo artifact)
		{
			if (!_longTextHelper.IsExtractedTextMissing())
			{
				return new List<LongText>();
			}

			if (_longTextHelper.IsTextTooLong(artifact, LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME))
			{
				return CreateTooLongTextForIpro(artifact);
			}
			return CreateLongTextForIpro(artifact);
		}

		private IList<LongText> CreateTooLongTextForIpro(ObjectExportInfo artifact)
		{
			string tempLocation = Path.GetTempFileName();
			int extractedTextFieldId = _longTextHelper.GetFieldArtifactId(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
			TextExportRequest textExportRequest = TextExportRequest.CreateRequestForFullText(artifact, extractedTextFieldId, tempLocation);
			LongText longText = LongText.CreateFromMissingValue(artifact.ArtifactID, extractedTextFieldId, textExportRequest);
			return new List<LongText> {longText};
		}

		private IList<LongText> CreateLongTextForIpro(ObjectExportInfo artifact)
		{
			string longTextValue = _longTextHelper.GetTextFromField(artifact, LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
			int extractedTextFieldId = _longTextHelper.GetFieldArtifactId(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
			LongText longText = LongText.CreateFromExistingValue(artifact.ArtifactID, extractedTextFieldId, longTextValue);
			return new List<LongText> {longText};
		}
	}
}