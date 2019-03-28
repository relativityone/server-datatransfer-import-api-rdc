using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public interface IRepositoryBuilder
	{
		void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken);
	}
}