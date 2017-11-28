using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public class LoadFileDestinationPath : DestinationPath
	{
		public LoadFileDestinationPath(ExportFile exportSettings) : base(exportSettings)
		{
		}

		public override string Path => FormatPath($".{ExportSettings.LoadFileExtension}");

		public override Encoding Encoding => ExportSettings.LoadFileEncoding;
	}
}