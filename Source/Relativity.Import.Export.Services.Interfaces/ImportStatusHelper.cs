using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Import.Export.Services
{
	using Microsoft.VisualBasic;

	public static class ImportStatusHelper
	{
		public static string ConvertToMessageLineInCell(string message)
		{
			return string.Format(" - {0}{1}", message, Strings.ChrW(10));
		}
	}
}
