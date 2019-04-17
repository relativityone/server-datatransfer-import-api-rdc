using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Import.Export.Services
{
	public static class SqlNameHelper
	{

		// TODO: remove method and change usages to kCura.Utility.SqlNameHelper
		/// <summary>
		/// 		''' This method is obsolete. Use "kCura.Utility.SqlNameHelper.GetSqlFriendlyName" instead.
		/// 		''' </summary>
		/// 		''' <param name="displayName"></param>
		/// 		''' <returns></returns>
		public static string GetSqlFriendlyName(string displayName)
		{
			return System.Text.RegularExpressions.Regex.Replace(displayName == null ? string.Empty : displayName, @"[\W]+", "");
		}
	}

}
