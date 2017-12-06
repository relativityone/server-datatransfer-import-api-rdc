using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public interface IBatchInitialization
	{
		void PrepareBatch(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}