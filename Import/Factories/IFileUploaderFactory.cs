namespace kCura.WinEDDS.Core.Import.Factories
{
	public interface IFileUploaderFactory
	{
		IFileUploader CreateNativeFileUploader();

		IFileUploader CreateBcpFileUploader();
	}
}