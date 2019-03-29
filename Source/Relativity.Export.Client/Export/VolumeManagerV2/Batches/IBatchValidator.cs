namespace Relativity.Export.VolumeManagerV2.Batches
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface IBatchValidator
	{
		void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken);
	}
}