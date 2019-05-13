namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	public interface IStateful
	{
		void SaveState();
		void RestoreLastState();
	}
}