using System.IO;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class PrefixFilePathProvider : IFilePathProvider
	{
		private readonly ExportFile _exportSettings;
		private readonly RelativeFilePathProvider _relativeFilePathProvider;

		public PrefixFilePathProvider(ExportFile exportSettings, RelativeFilePathProvider relativeFilePathProvider)
		{
			_exportSettings = exportSettings;
			_relativeFilePathProvider = relativeFilePathProvider;
		}

		public string GetPathForLoadFile(string filePath)
		{
			string relativePath = _relativeFilePathProvider.GetPathForLoadFile(filePath);
			return Path.Combine(_exportSettings.FilePrefix, relativePath);
		}
	}
}