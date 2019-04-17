using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Import.Export.Services
{
	public static class ProductionDocumentBatesHelper
	{
		public static object[] ToSerializableObjectArray(System.Data.DataRow input)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}

			object[] retval = new object[3];
			retval[0] = input["DocumentArtifactID"];
			retval[1] = System.Text.Encoding.Unicode.GetBytes(System.Convert.ToString(input["BeginBates"]));
			retval[2] = input["ProductionArtifactID"];
			return retval;
		}

		public static void CleanupSerialization(object[][] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}

			// TODO: investigate Parallel.ForEach
			foreach (object[] item in input)
				item[1] = System.Text.Encoding.Unicode.GetString((byte[])item[1]);
		}
	}
}
