namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers
{
	using System.Threading;

	public interface ILongTextEncodingConverterFactory
	{
		IFileDownloadSubscriber Create(CancellationToken cancellationToken);
	}
}
