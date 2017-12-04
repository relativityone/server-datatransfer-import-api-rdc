using System.IO;
using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextInFile : LongText
	{
		public override TextReader GetLongText()
		{
			//TODO encoding?
			return new StreamReader(Location, Encoding.Default);
		}
	}
}