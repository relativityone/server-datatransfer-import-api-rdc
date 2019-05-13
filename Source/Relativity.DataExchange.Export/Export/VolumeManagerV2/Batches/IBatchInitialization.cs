namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IBatchInitialization
	{
		void PrepareBatch(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken);
	}
}