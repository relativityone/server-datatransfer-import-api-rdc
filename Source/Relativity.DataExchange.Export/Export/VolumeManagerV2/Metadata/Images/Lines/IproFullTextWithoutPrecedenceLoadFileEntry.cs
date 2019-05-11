namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS;

	using Relativity.Logging;

	public class IproFullTextWithoutPrecedenceLoadFileEntry : IproFullTextLoadFileEntry
	{
		public IproFullTextWithoutPrecedenceLoadFileEntry(IFieldService fieldService, LongTextHelper longTextHelper, ILog logger, IFullTextLineWriter fullTextLineWriter)
			: base(fieldService, longTextHelper, logger, fullTextLineWriter)
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