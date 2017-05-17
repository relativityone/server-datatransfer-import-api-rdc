namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IErrorManager
	{
		void ExportErrorFile(string exportLocation);
		void ExportErrorReport(string exportLocation);
		void ExportErrors(string exportLocation, string loadFilePath);
	}
}