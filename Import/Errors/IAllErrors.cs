using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IAllErrors : IErrorFile
	{
		IList<LineError> GetAllErrors();

		bool HasErrors();
	}
}