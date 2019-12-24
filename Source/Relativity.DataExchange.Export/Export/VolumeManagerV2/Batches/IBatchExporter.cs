namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.WinEDDS.Exporters;

	public interface IBatchExporter
	{
		Task ExportAsync(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}