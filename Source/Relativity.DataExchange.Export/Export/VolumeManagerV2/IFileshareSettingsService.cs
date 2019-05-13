namespace Relativity.DataExchange.Export.VolumeManagerV2
{
	public interface IFileShareSettingsService
	{
		IRelativityFileShareSettings GetSettingsForFileshare(string fileUrl);
	}
}