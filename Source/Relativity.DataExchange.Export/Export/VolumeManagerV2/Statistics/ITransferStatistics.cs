namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Transfer;

	public interface ITransferStatistics
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}