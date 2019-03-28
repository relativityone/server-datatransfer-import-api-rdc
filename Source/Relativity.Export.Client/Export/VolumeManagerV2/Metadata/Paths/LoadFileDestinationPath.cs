using System.Text;
using kCura.WinEDDS.Exceptions;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths
{
	public class LoadFileDestinationPath : DestinationPath
	{
		public LoadFileDestinationPath(ExportFile exportSettings) : base(exportSettings)
		{
		}

		public override string Path => FormatPath($".{ExportSettings.LoadFileExtension}");

		public override Encoding Encoding => ExportSettings.LoadFileEncoding;
		public override FileWriteException.DestinationFile DestinationFileType => FileWriteException.DestinationFile.Load;
	}
}