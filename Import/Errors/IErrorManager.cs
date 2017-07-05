namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IErrorManager
	{
		bool HasErrors { get; }
		void ExportErrorFile(string exportLocation);
		void ExportErrorReport(string exportLocation);
		void ExportErrors(string exportLocation, string loadFilePath);
	}
}