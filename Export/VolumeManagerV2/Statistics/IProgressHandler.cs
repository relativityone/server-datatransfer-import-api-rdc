using kCura.WinEDDS.TApi;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface IProgressHandler
	{
		void Attach(TapiBridge tapiBridge);
		void Detach();
	}
}