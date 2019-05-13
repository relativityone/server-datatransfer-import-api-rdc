namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System.Threading;

	public interface ILongTextTapiBridgeFactory
	{
		IDownloadTapiBridge Create(CancellationToken token);
	}
}