using System.IO;
using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public interface IStreamFactory
	{
		StreamWriter Create(StreamWriter currentStreamWriter, long lastStreamWriterPosition, string path, Encoding encoding, bool append);
	}
}