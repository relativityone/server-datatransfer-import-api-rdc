using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IExportTapiBridgeFactory
	{
		IDownloadTapiBridge CreateForImages(CancellationToken token);
		IDownloadTapiBridge CreateForLongText(CancellationToken token);
		IDownloadTapiBridge CreateForNatives(CancellationToken token);
	}
}