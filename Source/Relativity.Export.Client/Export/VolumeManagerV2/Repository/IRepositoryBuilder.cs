namespace Relativity.Export.VolumeManagerV2.Repository
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IRepositoryBuilder
	{
		void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken);
	}
}