namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using Relativity.DataExchange.Transfer;

	public interface ILongTextEncodingConverter
	{
		void Subscribe(ITapiBridge tapiBridge);
		void Unsubscribe(ITapiBridge tapiBridge);
		void WaitForConversionCompletion();
	}
}