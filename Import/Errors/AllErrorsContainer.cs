using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;

namespace kCura.WinEDDS.Core.Import.Errors
{
	public class AllErrorsContainer : IErrorContainer, IAllErrors
	{
		private readonly IPathHelper _pathHelper;
		private readonly ConcurrentBag<LineError> _errors;

		public AllErrorsContainer(IPathHelper pathHelper)
		{
			_pathHelper = pathHelper;
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

		public string WriteErrorsToTempFile()
		{
			var filePath = _pathHelper.GetTempFileName();

			using (var fileWriter = new StreamWriter(filePath, true, Encoding.Default))
			{
				foreach (var lineError in GetAllErrors())
				{
					fileWriter.WriteLine(
						$"{CsvFormat(lineError.LineNumber.ToString())},{CsvFormat(lineError.Message)},{CsvFormat(lineError.Identifier)},{CsvFormat(lineError.ErrorType.ToString())}");
				}
			}

			return filePath;
		}

		private string CsvFormat(string s)
		{
			return ControlChars.Quote + s.Replace(ControlChars.Quote.ToString(), ControlChars.Quote + ControlChars.Quote.ToString()) + ControlChars.Quote;
		}
	}
}