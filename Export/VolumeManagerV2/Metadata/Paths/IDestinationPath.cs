using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths
{
	public interface IDestinationPath
	{
		string Path { get; }
		Encoding Encoding { get; }
	}
}