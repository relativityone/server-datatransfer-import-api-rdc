using System.Collections.Generic;
using System.IO;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextFieldBuilder : ILongTextBuilder
	{
		private readonly IFieldService _fieldService;
		private readonly LongTextHelper _longTextHelper;

		public LongTextFieldBuilder(IFieldService fieldService, LongTextHelper longTextHelper)
		{
			_fieldService = fieldService;
			_longTextHelper = longTextHelper;
		}

		public IList<LongText> CreateLongText(ObjectExportInfo artifact)
		{
			IList<LongText> longTexts = new List<LongText>();

			for (int i = 0; i < _fieldService.GetColumns().Length; i++)
			{
				ViewFieldInfo field = _fieldService.GetColumns()[i];
				if (_longTextHelper.IsTextTooLong(artifact, field.AvfColumnName))
				{
					if (!(field is CoalescedTextViewField))
					{
						string tempLocation = Path.GetTempFileName();
						TextExportRequest exportRequest = TextExportRequest.CreateRequestForLongText(artifact, field.FieldArtifactId, tempLocation);
						LongText longText = LongText.CreateFromMissingValue(artifact.ArtifactID, field.FieldArtifactId, exportRequest);
						longTexts.Add(longText);
					}
				}
				else
				{
					string longTextValue = _longTextHelper.GetTextFromField(artifact, field.AvfColumnName);
					LongText longText = LongText.CreateFromExistingValue(artifact.ArtifactID, field.FieldArtifactId, longTextValue);
					longTexts.Add(longText);
				}
			}

			return longTexts;
		}
	}
}