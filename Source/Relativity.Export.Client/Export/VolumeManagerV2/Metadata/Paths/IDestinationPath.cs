using System.Text;
using kCura.WinEDDS.Exceptions;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths
{
	public interface IDestinationPath
	{
		string Path { get; }
		Encoding Encoding { get; }
		FileWriteException.DestinationFile DestinationFileType { get; }
	}
}