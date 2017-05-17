using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ErrorMessageContainer : IErrorContainer, IErrorMessages
	{
		private readonly ConcurrentBag<LineError> _errors;

		public ErrorMessageContainer()
		{
			_errors = new ConcurrentBag<LineError>();
		}

		public void WriteError(LineError lineError)
		{
			_errors.Add(lineError);
		}

		public bool HasErrors()
		{
			return !_errors.IsEmpty;
		}

		public IList<LineError> GetAllErrors()
		{
			return _errors.OrderBy(x => x.LineNumber).ToList();
		}
	}
}