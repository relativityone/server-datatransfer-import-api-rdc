using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class ImageFilePathProvider : FilePathProvider
	{
		public ImageFilePathProvider(ILabelManagerForArtifact labelManagerForArtifact,
			ExportFile exportSettings, IDirectoryHelper directoryHelper, ILog logger) : base(labelManagerForArtifact, exportSettings,
			directoryHelper, logger)
		{
		}

		protected override string GetSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return LabelManagerForArtifact.GetImageSubdirectoryLabel(objectExportInfoArtifactId);
		}
	}
}