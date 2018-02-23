using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IExportTapiBridgeFactory
	{
		IDownloadTapiBridge CreateForLongText(CancellationToken token);
		IDownloadTapiBridge CreateForFiles(CancellationToken token);
	}
}