using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class IproFullTextWithoutPrecedenceLoadFileEntry : IproFullTextLoadFileEntry
	{
		public IproFullTextWithoutPrecedenceLoadFileEntry(ExportFile exportSettings, IFieldService fieldService, LongTextHelper longTextHelper, ILog logger)
			: base(exportSettings, fieldService, longTextHelper, logger)
		{
		}

		protected override int GetTextSourceFieldId(ObjectExportInfo artifact)
		{
			return LongTextHelper.GetFieldArtifactId(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME);
		}

		protected override string GetTextColumnName()
		{
			return LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME;
		}
	}
}