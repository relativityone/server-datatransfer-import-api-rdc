namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Transfer;

	public interface ITransferClientHandler
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach(ITapiBridge tapiBridge);
	}
}