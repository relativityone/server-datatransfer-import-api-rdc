using System.IO;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public interface IFullTextLineWriter
	{
		void WriteLine(string batesNumber, long pageOffset, IRetryableStreamWriter writer, TextReader textReader, CancellationToken token);
	}
}