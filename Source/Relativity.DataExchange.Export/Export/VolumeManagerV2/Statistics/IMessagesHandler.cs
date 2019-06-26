namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Transfer;

	public interface IMessagesHandler
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}