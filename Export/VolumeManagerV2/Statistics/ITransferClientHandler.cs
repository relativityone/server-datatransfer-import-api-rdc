using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface ITransferClientHandler
	{
		void Attach(TapiBridge tapiBridge);
		void Detach();
	}
}