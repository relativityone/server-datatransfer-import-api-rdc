using System;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IFileTapiBridgePool : IDisposable
	{
		IDownloadTapiBridge Request(IRelativityFileShareSettings fileshareSettings, CancellationToken token);

		void Release(IDownloadTapiBridge bridge);
	}
}