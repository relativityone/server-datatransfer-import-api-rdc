namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Threading;

	public interface ILongTextDownloadTapiBridgeFactory
	{
		IDownloadTapiBridge Create(CancellationToken token);
	}
}