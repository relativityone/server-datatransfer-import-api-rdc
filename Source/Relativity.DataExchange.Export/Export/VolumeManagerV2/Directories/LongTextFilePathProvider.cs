namespace Relativity.Export.VolumeManagerV2.Directories
{
	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	using kCura.WinEDDS;

	public class LongTextFilePathProvider : FilePathProvider
	{
		public LongTextFilePathProvider(ILabelManagerForArtifact labelManagerForArtifact,
			ExportFile exportSettings, IDirectory directoryHelper, ILog logger) : base(labelManagerForArtifact, exportSettings,
			directoryHelper, logger)
		{
		}

		protected override string GetSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return LabelManagerForArtifact.GetTextSubdirectoryLabel(objectExportInfoArtifactId);
		}
	}
}