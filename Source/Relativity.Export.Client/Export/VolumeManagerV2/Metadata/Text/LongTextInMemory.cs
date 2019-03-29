namespace Relativity.Export.VolumeManagerV2.Metadata.Text
{
	using System.IO;

	public class LongTextInMemory : LongText
	{
		public override TextReader GetLongText()
		{
			return new StringReader(LongTextValue);
		}
	}
}