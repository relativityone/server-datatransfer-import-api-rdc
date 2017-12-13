using System.IO;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextInMemory : LongText
	{
		public override TextReader GetLongText()
		{
			return new StringReader(LongTextValue);
		}
	}
}