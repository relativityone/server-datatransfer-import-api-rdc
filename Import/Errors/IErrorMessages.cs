using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public interface IErrorMessages
	{
		IList<LineError> GetAllErrors();
	}
}