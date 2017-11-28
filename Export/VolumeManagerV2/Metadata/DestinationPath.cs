using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public abstract class DestinationPath : IDestinationPath
	{
		protected readonly ExportFile ExportSettings;

		protected DestinationPath(ExportFile exportSettings)
		{
			ExportSettings = exportSettings;
		}

		protected string FormatPath(string extension)
		{
			//TODO change to Path.Combine()
			return $"{ExportSettings.FolderPath.TrimEnd('\\')}\\{ExportSettings.LoadFilesPrefix}_export{extension}";
		}

		public abstract string Path { get; }
		public abstract Encoding Encoding { get; }
	}
}