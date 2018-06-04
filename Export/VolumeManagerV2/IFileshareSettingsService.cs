namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IFileshareSettingsService
	{
		RelativityFileShareSettings GetSettingsForFileshare(string fileUrl);
	}
}
