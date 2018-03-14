using System.Threading;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public interface IExportTapiBridgeFactory
	{
		IDownloadTapiBridge CreateForLongText(CancellationToken token);
		IDownloadTapiBridge CreateForFiles(AsperaCredential asperaCredentials, CancellationToken token);
	}
}