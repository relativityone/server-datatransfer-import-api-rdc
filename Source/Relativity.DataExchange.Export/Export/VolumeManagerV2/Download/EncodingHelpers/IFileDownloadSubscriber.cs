namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Threading;
	
	public interface IFileDownloadSubscriber : IDisposable
	{
		void SubscribeForDownloadEvents(IFileTransferProducer fileTransferProducer, CancellationToken token);
	}
}