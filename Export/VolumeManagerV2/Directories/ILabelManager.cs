namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public interface ILabelManager
	{
		string GetCurrentImageSubdirectoryLabel();
		string GetCurrentNativeSubdirectoryLabel();
		string GetCurrentTextSubdirectoryLabel();
		string GetCurrentVolumeLabel();
	}
}