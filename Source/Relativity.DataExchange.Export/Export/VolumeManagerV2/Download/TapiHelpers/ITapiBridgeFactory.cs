namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using Relativity.DataExchange.Transfer;

	public interface ITapiBridgeFactory
	{
		ITapiBridge Create();
	}
}