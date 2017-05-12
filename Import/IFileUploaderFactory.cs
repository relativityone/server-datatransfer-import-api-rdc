namespace kCura.WinEDDS.Core.Import
{
	public interface IFileUploaderFactory
	{
		IFileUploader CreateNativeFileUploader();

		IFileUploader CreateBcpFileUploader();
	}
}