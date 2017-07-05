using System.Collections.Generic;
using System.Linq;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorContainerComposite : IErrorContainer
	{
		private readonly IList<IErrorContainer> _errorContainers;

		public ErrorContainerComposite(IList<IErrorContainer> errorContainers)
		{
			_errorContainers = errorContainers;
		}

		public void WriteError(LineError lineError)
		{
			foreach (var errorContainer in _errorContainers)
			{
				errorContainer.WriteError(lineError);
			}
		}

		public bool HasErrors()
		{
			return _errorContainers.Any(x => x.HasErrors());
		}
	}
}