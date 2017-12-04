using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class IproFullTextWithPrecedenceLoadFileEntry : IproFullTextLoadFileEntry
	{
		public IproFullTextWithPrecedenceLoadFileEntry(ExportFile exportSettings, IFieldService fieldService, LongTextHelper longTextHelper, ILog logger) :
			base(exportSettings, fieldService, longTextHelper, logger)
		{
		}

		protected override int GetTextSourceFieldId(ObjectExportInfo artifact)
		{
			string textSourceColumnName = Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME;
			int fieldIndex = FieldService.GetOrdinalIndex(textSourceColumnName);
			return (int) artifact.Metadata[fieldIndex];
		}

		protected override string GetTextColumnName()
		{
			return Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME;
		}
	}
}