namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Threading.Tasks;

	public interface IFileDownloadSubscriber : IDisposable
	{
		void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer);
		Task WaitForConversionCompletion();
	}
}