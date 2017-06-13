namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IErrorContainer
	{
		void WriteError(LineError lineError);

		bool HasErrors();
	}
}