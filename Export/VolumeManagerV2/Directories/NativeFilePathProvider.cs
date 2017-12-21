using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class NativeFilePathProvider : FilePathProvider
	{
		public NativeFilePathProvider(ILabelManager labelManager, ExportFile exportSettings, IDirectoryHelper directoryHelper, ILog logger) : base(labelManager, exportSettings,
			directoryHelper, logger)
		{
		}

		protected override string GetSubdirectoryLabel()
		{
			return LabelManager.GetCurrentNativeSubdirectoryLabel();
		}
	}
}