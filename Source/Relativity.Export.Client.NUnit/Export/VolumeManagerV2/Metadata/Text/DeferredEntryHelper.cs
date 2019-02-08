using System.IO;
using System.Text;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Text
{
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