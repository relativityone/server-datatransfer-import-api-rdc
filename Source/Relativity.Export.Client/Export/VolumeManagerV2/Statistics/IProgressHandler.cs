using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface IProgressHandler
	{
		void Attach(ITapiBridge tapiBridge);
		void Detach();
	}
}