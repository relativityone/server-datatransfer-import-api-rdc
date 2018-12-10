namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IFileShareSettingsService
	{
		IRelativityFileShareSettings GetSettingsForFileshare(string fileUrl);
	}
}
