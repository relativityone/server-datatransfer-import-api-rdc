namespace Relativity.DataExchange.Export.VolumeManagerV2.Directories
{
	public interface ILabelManager
	{
		string GetCurrentImageSubdirectoryLabel();
		string GetCurrentNativeSubdirectoryLabel();
		string GetCurrentTextSubdirectoryLabel();
		string GetCurrentPdfSubdirectoryLabel();
		string GetCurrentVolumeLabel();
	}
}