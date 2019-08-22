namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System.Threading.Tasks;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;

	public interface IFileDownloadSubscriber
	{
		void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer);
		Task WaitForConversionCompletion();
	}
}