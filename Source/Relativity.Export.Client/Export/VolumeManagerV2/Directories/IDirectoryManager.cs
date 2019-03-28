using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public interface IDirectoryManager
	{
		void MoveNext(VolumePredictions volumePredictions);
	}
}