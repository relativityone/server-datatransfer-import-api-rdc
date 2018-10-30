namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IFileshareSettingsService
	{
		IRelativityFileShareSettings GetSettingsForFileshare(string fileUrl);
	}
}
