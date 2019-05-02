using System;

namespace Relativity.Import.Export.Services
{
	public static class ProductionDocumentBatesHelper
	{
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