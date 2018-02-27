using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public interface IBatchExporter
	{
		void Export(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}