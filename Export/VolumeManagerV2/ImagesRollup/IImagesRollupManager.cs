using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public interface IImagesRollupManager
	{
		void RollupImagesForArtifacts(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}