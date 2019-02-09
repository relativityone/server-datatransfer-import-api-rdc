// -----------------------------------------------------------------------------------------------------
// <copyright file="DeferredEntryHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.IO;
    using System.Text;

    using kCura.WinEDDS.LoadFileEntry;

    public static class DeferredEntryHelper
	{
		public static string GetTextFromEntry(this DeferredEntry entry)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StreamWriter writer = new StreamWriter(memoryStream, Encoding.Default);

				entry.Write(ref writer);

				writer.Flush();
				writer.Dispose();

				return Encoding.Default.GetString(memoryStream.ToArray());
			}
		}
	}
}