namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text
{
	using System.IO;

	public class LongTextInFile : LongText
	{
		public override TextReader GetLongText()
		{
			return new StreamReader(Location, SourceEncoding);
		}
	}
}