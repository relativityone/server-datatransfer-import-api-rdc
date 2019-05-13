namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;

	public interface ITransferStatistics
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}