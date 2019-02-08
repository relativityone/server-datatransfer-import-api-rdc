using System.IO;
using System.Text;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Natives
{
	public class LoadFileTestHelpers
	{
		public static string GetStringFromEntry(DeferredEntry entry)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.ASCII);
			entry.Write(ref streamWriter);
			streamWriter.Flush();

			string actualValue = Encoding.ASCII.GetString(memoryStream.ToArray());

			return actualValue;
		}

		public static string FormatPathEntry(string s, char quoteDelimiter, char recordDelimiter)
		{
			return $"{recordDelimiter}{quoteDelimiter}{s}{quoteDelimiter}";
		}
	}
}