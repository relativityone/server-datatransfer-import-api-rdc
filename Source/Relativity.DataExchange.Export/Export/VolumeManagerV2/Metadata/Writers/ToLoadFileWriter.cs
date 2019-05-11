namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System.IO;
	using System.Text;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	public abstract class ToLoadFileWriter : ILongTextEntryWriter
	{
		public abstract void WriteLongTextFileToDatFile(StreamWriter fileWriter, string longTextPath, Encoding encoding);

		protected void WriteLongText(TextReader source, TextWriter fileWriter, ILongTextStreamFormatter formatter)
		{
			int c = source.Read();

			while (c != -1)
			{
				formatter.TransformAndWriteCharacter(c, fileWriter);
				c = source.Read();
			}
		}
	}
}