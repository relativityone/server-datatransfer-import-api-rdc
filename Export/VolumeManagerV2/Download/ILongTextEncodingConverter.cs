using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface ILongTextEncodingConverter
	{
		void StartListening(ITapiBridge tapiBridge);
		void StopListening(ITapiBridge tapiBridge);
		void WaitForConversionCompletion();
	}
}