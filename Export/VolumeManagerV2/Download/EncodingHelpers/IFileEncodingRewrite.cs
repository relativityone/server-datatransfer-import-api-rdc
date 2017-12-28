using System.Text;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers
{
	public interface IFileEncodingRewrite
	{
		void RewriteFile(string filePath, string tmpFilePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken);
	}
}