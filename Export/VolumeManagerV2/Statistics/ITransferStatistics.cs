using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface ITransferStatistics
	{
		void Attach(TapiBridge tapiBridge);
		void Detach();
	}
}