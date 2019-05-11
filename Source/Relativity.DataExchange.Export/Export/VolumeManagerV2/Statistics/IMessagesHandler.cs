namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;

	public interface IMessagesHandler
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}