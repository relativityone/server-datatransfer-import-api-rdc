using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface ITransferStatistics
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}