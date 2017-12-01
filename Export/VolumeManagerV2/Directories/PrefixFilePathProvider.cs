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
			//TODO I don't like this trimming
			return Path.Combine(_exportSettings.FilePrefix, relativePath.TrimStart('.').TrimStart('\\'));
		}
	}
}