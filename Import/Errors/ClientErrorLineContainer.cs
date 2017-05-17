using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ClientErrorLineContainer : IErrorContainer, IClientErrors
	{
		private readonly ConcurrentBag<int> _errorLines;

		public ClientErrorLineContainer()
		{
			_errorLines = new ConcurrentBag<int>();
		}

		public void WriteError(LineError lineError)
		{
			if (lineError.ErrorType == ErrorType.client)
			{
				_errorLines.Add(lineError.LineNumber);
			}
		}

		public bool HasErrors()
		{
			return !_errorLines.IsEmpty;
		}

		public IList<int> GetClientErrorLines()
		{
			return _errorLines.Distinct().OrderBy(x => x).ToList();
		}
	}
}