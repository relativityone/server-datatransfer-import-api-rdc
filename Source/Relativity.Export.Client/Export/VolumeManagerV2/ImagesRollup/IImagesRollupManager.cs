namespace Relativity.Export.VolumeManagerV2.ImagesRollup
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IImagesRollupManager
	{
		void RollupImagesForArtifacts(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}