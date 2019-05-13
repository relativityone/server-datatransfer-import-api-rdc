namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	public interface IExportFileValidator
	{
		bool CanExport(string destinationLocation, string warningUserMessage);
	}
}