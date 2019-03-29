namespace Relativity.Export.VolumeManagerV2.Directories
{
	using kCura.WinEDDS;

	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	public class NativeFilePathProvider : FilePathProvider
	{
		public NativeFilePathProvider(ILabelManagerForArtifact labelManagerForArtifact,
			ExportFile exportSettings, IDirectory directoryHelper, ILog logger) : base(labelManagerForArtifact, exportSettings,
			directoryHelper, logger)
		{
		}

		protected override string GetSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return LabelManagerForArtifact.GetNativeSubdirectoryLabel(objectExportInfoArtifactId);
		}
	}
}