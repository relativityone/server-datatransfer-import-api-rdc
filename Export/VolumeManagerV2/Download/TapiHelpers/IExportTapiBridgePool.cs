using System;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IExportTapiBridgePool : IDisposable
	{
		IDownloadTapiBridge RequestForFiles(RelativityFileShareSettings fileshareSettings, CancellationToken token);

		IDownloadTapiBridge RequestForLongText(CancellationToken token);

		void ReleaseFiles(IDownloadTapiBridge tapiBridge);

		void ReleaseLongText(IDownloadTapiBridge tapiBridge);
	}
}