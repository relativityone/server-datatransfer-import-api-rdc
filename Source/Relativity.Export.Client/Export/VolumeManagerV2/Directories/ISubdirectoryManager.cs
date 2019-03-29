namespace Relativity.Export.VolumeManagerV2.Directories
{
	using kCura.WinEDDS.Exporters;

	public interface ISubdirectoryManager
	{
		void MoveNext(VolumePredictions volumePredictions);

		void RestartSubdirectoryCounting();
	}
}