using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextFromFieldBuilder : ILongTextBuilder
	{
		private readonly IFieldService _fieldService;
		private readonly LongTextHelper _longTextHelper;
		private readonly ILog _logger;

		public LongTextFromFieldBuilder(IFieldService fieldService, LongTextHelper longTextHelper, ILog logger)
		{
			_fieldService = fieldService;
			_longTextHelper = longTextHelper;
			_logger = logger;
		}

		public IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating LongText from fields for artifact {artifactId}.", artifact.ArtifactID);
			IList<LongText> longTexts = new List<LongText>();

			for (int i = 0; i < _fieldService.GetColumns().Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return longTexts;
				}
				ViewFieldInfo field = _fieldService.GetColumns()[i];
				if (_longTextHelper.IsLongTextField(field))
				{
					_logger.LogVerbose("Creating LongText from field {field}.", field.AvfColumnName);
					if (_longTextHelper.IsTextTooLong(artifact, field.AvfColumnName))
					{
						//We can assume that CoalescedTextViewField fields have been handled in LongTextPrecedenceBuilder
						if (!(field is CoalescedTextViewField))
						{
							HandleTooLongTextWithoutPrecedence(artifact, field, longTexts);
						}
					}
					else
					{
						HandleLongText(artifact, field, longTexts);
					}
				}
			}

			return longTexts;
		}

		private void HandleTooLongTextWithoutPrecedence(ObjectExportInfo artifact, ViewFieldInfo field, IList<LongText> longTexts)
		{
			_logger.LogVerbose("Creating LongText with missing value.");
			string tempLocation = Path.GetTempFileName();
			LongTextExportRequest exportRequest = LongTextExportRequest.CreateRequestForLongText(artifact, field.FieldArtifactId, tempLocation);
			Encoding sourceEncoding = _longTextHelper.GetLongTextFieldFileEncoding(field);
			LongText longText = LongText.CreateFromMissingValue(artifact.ArtifactID, field.FieldArtifactId, exportRequest, sourceEncoding);
			longTexts.Add(longText);
		}

		private void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, IList<LongText> longTexts)
		{
			_logger.LogVerbose("Creating LongText with existing value.");
			string longTextValue = _longTextHelper.GetTextFromField(artifact, field.AvfColumnName);
			LongText longText = LongText.CreateFromExistingValue(artifact.ArtifactID, field.FieldArtifactId, longTextValue);
			longTexts.Add(longText);
		}
	}
}