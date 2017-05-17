using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IClientErrors
	{
		IList<int> GetClientErrorLines();
	}
}