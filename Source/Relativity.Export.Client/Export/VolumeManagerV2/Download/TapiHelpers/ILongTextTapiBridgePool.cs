using System;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface ILongTextTapiBridgePool : IDisposable
	{
		IDownloadTapiBridge Request(CancellationToken token);

		void Release(IDownloadTapiBridge bridge);
	}
}