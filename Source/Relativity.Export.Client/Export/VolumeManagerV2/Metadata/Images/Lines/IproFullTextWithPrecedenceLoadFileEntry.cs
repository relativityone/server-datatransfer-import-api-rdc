namespace Relativity.Export.VolumeManagerV2.Metadata.Images.Lines
{
	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Logging;

	using Constants = Relativity.Export.Constants;

	public class IproFullTextWithPrecedenceLoadFileEntry : IproFullTextLoadFileEntry
	{
		public IproFullTextWithPrecedenceLoadFileEntry(IFieldService fieldService, LongTextHelper longTextHelper, ILog logger, IFullTextLineWriter fullTextLineWriter) :
			base(fieldService, longTextHelper, logger, fullTextLineWriter)
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