using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class IproFullTextWithoutPrecedenceLoadFileEntry : IproFullTextLoadFileEntry
	{
		public IproFullTextWithoutPrecedenceLoadFileEntry(ExportFile exportSettings, IFieldService fieldService, DownloadedTextFilesRepository downloadedTextFilesRepository)
			: base(exportSettings, fieldService, downloadedTextFilesRepository)
		{
		}

		protected override int GetTextSourceFieldId(ObjectExportInfo artifact)
		{
			int fieldIndex = FieldService.GetOrdinalIndex(FieldUtils.EXTRACTED_TEXT_COLUMN_NAME);
			if (fieldIndex >= FieldService.GetColumns().Count)
			{
				return FieldUtils.EXTRACTED_TEXT_FIELD_ID;
			}
			ViewFieldInfo fieldInfo = (ViewFieldInfo) FieldService.GetColumns()[fieldIndex];
			return fieldInfo.FieldArtifactId;
		}

		protected override string GetTextColumnName()
		{
			return FieldUtils.EXTRACTED_TEXT_COLUMN_NAME;
		}
	}
}