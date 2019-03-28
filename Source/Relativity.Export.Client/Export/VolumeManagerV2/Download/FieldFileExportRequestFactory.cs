using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FieldFileExportRequestFactory
	{
		private readonly ExportFile _exportSettings;

		public FieldFileExportRequestFactory(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public FieldFileExportRequest Create(ObjectExportInfo artifact, string destinationLocation)
		{
			return new FieldFileExportRequest(artifact, _exportSettings.FileField.FieldID, destinationLocation);
		}
	}
}