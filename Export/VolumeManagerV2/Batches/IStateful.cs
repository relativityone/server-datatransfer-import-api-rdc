namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public interface IStateful
	{
		void SaveState();
		void RestoreLastState();
	}
}