using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class ClientErrorLineContainer : IErrorContainer, IClientErrors
	{
		private readonly IPathHelper _pathHelper;
		private readonly ConcurrentBag<int> _errorLines;

		public ClientErrorLineContainer(IPathHelper pathHelper)
		{
			_pathHelper = pathHelper;
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

		public string WriteErrorsToTempFile()
		{
			var filePath = _pathHelper.GetTempFileName();

			using (var writer = new StreamWriter(filePath, true, Encoding.Default))
			{
				foreach (var errorLine in GetClientErrorLines())
				{
					writer.WriteLine(errorLine);
				}
			}

			return filePath;
		}
	}
}