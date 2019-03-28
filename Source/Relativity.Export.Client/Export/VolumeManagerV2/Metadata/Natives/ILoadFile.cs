using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public interface ILoadFile
	{
		void Create(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}