namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;

	public interface ILongTextEncodingConverter
	{
		void StartListening(ITapiBridge tapiBridge);
		void StopListening(ITapiBridge tapiBridge);
		void WaitForConversionCompletion();
	}
}