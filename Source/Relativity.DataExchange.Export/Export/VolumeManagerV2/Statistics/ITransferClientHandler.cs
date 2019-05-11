using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;

namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	public interface ITransferClientHandler
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}