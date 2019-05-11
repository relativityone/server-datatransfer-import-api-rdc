namespace Relativity.DataExchange.Export.VolumeManagerV2.Directories
{
	using kCura.WinEDDS;

	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	public class ImageFilePathProvider : FilePathProvider
	{
		public ImageFilePathProvider(ILabelManagerForArtifact labelManagerForArtifact,
			ExportFile exportSettings, IDirectory directoryHelper, ILog logger) : base(labelManagerForArtifact, exportSettings,
			directoryHelper, logger)
		{
		}

		protected override string GetSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return LabelManagerForArtifact.GetImageSubdirectoryLabel(objectExportInfoArtifactId);
		}
	}
}