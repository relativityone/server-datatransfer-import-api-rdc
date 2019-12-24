namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	public interface ILongTextFileDownloadSubscriber
	{
		void RegisterSubscriber(IFileDownloadSubscriber fileDownloadSubscriber);
	}
}