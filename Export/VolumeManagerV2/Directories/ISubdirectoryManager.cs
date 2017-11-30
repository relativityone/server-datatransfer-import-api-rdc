using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public interface ISubdirectoryManager
	{
		void MoveNext(VolumePredictions volumePredictions);

		void RestartSubdirectoryCounting();
	}
}