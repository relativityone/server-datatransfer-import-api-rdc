namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IFileDownloadSubscriber : IDisposable
	{
		void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer, CancellationToken token);

		Task WaitForConversionCompletion();
	}
}