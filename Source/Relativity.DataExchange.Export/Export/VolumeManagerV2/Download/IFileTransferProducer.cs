using System;
namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	public interface IFileTransferProducer
	{
		IObservable<string> FileDownloaded { get; }
	}
}
