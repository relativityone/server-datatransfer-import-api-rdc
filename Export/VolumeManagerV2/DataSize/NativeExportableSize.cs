using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize
{
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