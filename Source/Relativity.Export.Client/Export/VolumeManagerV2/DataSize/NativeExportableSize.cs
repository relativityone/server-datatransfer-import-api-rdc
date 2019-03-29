namespace Relativity.Export.VolumeManagerV2.DataSize
{
	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	public class NativeExportableSize
	{
		private readonly ExportFile _exportSettings;

		public NativeExportableSize(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public void CalculateNativesSize(VolumePredictions volumeSize)
		{
			bool areNativeFilesBeingExported = !_exportSettings.ExportNative || !_exportSettings.VolumeInfo.CopyNativeFilesFromRepository;
			if (areNativeFilesBeingExported)
			{
				volumeSize.NativeFileCount = 0;
				volumeSize.NativeFilesSize = 0;
			}
		}
	}
}