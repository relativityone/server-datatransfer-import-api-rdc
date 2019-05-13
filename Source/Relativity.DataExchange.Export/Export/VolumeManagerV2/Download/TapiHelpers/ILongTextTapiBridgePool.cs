namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Threading;

	public interface ILongTextTapiBridgePool : IDisposable
	{
		IDownloadTapiBridge Request(CancellationToken token);

		void Release(IDownloadTapiBridge bridge);
	}
}