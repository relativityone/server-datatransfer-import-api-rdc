using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextIproFullTextBuilder : ILongTextBuilder
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly ILog _logger;

		public LongTextIproFullTextBuilder(LongTextHelper longTextHelper, ILog logger)
		{
			_longTextHelper = longTextHelper;
			_logger = logger;
		}

		public IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Attempting to create LongText for IPRO Full text.");

			if (cancellationToken.IsCancellationRequested)
			{
				return Enumerable.Empty<LongText>().ToList();
			}

			if (_longTextHelper.IsTextTooLong(artifact, LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME))
			{
				return CreateTooLongTextForIpro(artifact).InList();
			}
			return CreateLongTextForIpro(artifact).InList();
		}

		private LongText CreateTooLongTextForIpro(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Creating LongText for IPRO Full text with missing value.");
			string tempLocation = Path.GetTempFileName();
			int extractedTextFieldId = _longTextHelper.GetFieldArtifactId(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
			LongTextExportRequest longTextExportRequest = LongTextExportRequest.CreateRequestForFullText(artifact, extractedTextFieldId, tempLocation);
			//TODO REL-188481 we're assuming Unicode for missing Extracted Text (the same assumption is in VolumeManager), but it can be wrong assumption
			LongText longText = LongText.CreateFromMissingValue(artifact.ArtifactID, extractedTextFieldId, longTextExportRequest, Encoding.Unicode);
			return longText;
		}

		private LongText CreateLongTextForIpro(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Creating LongText for IPRO Full text with existing value.");
			string longTextValue = _longTextHelper.GetTextFromField(artifact, LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
			int extractedTextFieldId = _longTextHelper.GetFieldArtifactId(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
			LongText longText = LongText.CreateFromExistingValue(artifact.ArtifactID, extractedTextFieldId, longTextValue);
			return longText;
		}
	}
}