namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Threading;

	public interface IFileTapiBridgePool : IDisposable
	{
		IDownloadTapiBridge Request(IRelativityFileShareSettings fileshareSettings, CancellationToken token);

		void Release(IDownloadTapiBridge bridge);
	}
}