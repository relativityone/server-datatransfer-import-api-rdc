using System.Text;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers
{
	public interface IFileEncodingConverter
	{
		void Convert(string filePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken);
	}
}