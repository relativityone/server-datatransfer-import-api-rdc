namespace Relativity.Export.VolumeManagerV2.Directories
{
	using kCura.WinEDDS.Exporters;

	public interface IDirectoryManager
	{
		void MoveNext(VolumePredictions volumePredictions);
	}
}