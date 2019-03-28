using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface ILongTextTapiBridgeFactory
	{
		IDownloadTapiBridge Create(CancellationToken token);
	}
}