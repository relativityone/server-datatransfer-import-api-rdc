using System;
namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Reactive.Subjects;

	public interface IFileTransferProducer
	{
		IObservable<string> FileDownloaded { get; }

		Subject<bool> FileDownloadCompleted { get; }
	}
}
