namespace Relativity.Export.VolumeManagerV2.Batches
{
	public interface IStateful
	{
		void SaveState();
		void RestoreLastState();
	}
}