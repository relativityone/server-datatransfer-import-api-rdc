namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using kCura.WinEDDS;

	using kCura.WinEDDS.Exporters;

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