namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System.Threading.Tasks;

	using Relativity.DataExchange.Transfer;

	public interface ILongTextEncodingConverter
	{
		void NotifyStartConversion();
		void NotifyStopConversion();
		Task WaitForConversionCompletion();

		void AddForConversion(string fileName);
	}
}