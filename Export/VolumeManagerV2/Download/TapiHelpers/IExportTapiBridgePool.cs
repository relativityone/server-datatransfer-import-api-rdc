using System;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IExportTapiBridgePool : IDisposable
	{
		IDownloadTapiBridge CreateForFiles(RelativityFileShareSettings fileshareSettings, CancellationToken token);

		IDownloadTapiBridge CreateForLongText(CancellationToken token);
	}
}