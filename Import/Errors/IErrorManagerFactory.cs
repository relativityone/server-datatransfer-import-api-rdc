namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IErrorManagerFactory
	{
		IErrorManager Create(IImportMetadata importMetadata);
	}
}