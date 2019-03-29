namespace Relativity.Export.VolumeManagerV2.Batches
{
	public interface IBatchState
	{
		void SaveState();
		void RestoreState();
	}
}