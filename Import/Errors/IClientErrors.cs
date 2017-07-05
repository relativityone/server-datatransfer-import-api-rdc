using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IClientErrors : IErrorFile
	{
		IList<int> GetClientErrorLines();

		bool HasErrors();
	}
}