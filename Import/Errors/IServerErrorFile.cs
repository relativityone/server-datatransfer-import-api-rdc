using kCura.Utility;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IServerErrorFile
	{
		void HandleServerErrors(GenericCsvReader reader);
	}
}