using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class ImageFilePathProvider : FilePathProvider
	{
		public ImageFilePathProvider(LabelManager labelManager, ExportFile exportSettings, IDirectoryHelper directoryHelper, ILog logger) : base(labelManager, exportSettings,
			directoryHelper, logger)
		{
		}

		protected override string GetSubdirectoryLabel()
		{
			return LabelManager.GetCurrentImageSubdirectoryLabel();
		}
	}
}