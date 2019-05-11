namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System.IO;
	using System.Text;

	public interface IStreamFactory
	{
		StreamWriter Create(StreamWriter currentStreamWriter, long lastStreamWriterPosition, string path, Encoding encoding, bool append);
	}
}