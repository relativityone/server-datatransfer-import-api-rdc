using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class IproFullTextWithPrecedenceLoadFileEntry : IproFullTextLoadFileEntry
	{
		public IproFullTextWithPrecedenceLoadFileEntry(ExportFile exportSettings, IFieldService fieldService, DownloadedTextFilesRepository downloadedTextFilesRepository) :
			base(exportSettings, fieldService, downloadedTextFilesRepository)
		{
		}

		protected override int GetTextSourceFieldId(ObjectExportInfo artifact)
		{
			string textSourceColumnName = Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME;
			int fieldIndex = FieldService.GetOrdinalIndex(textSourceColumnName);
			return (int) artifact.Metadata[fieldIndex];
		}

		protected override string GetTextColumnName()
		{
			return Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME;
		}
	}
}