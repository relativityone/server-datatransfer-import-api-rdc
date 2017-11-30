using System;
using System.IO;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class RelativeFilePathProvider : IFilePathProvider
	{
		private readonly ExportFile _exportSettings;

		public RelativeFilePathProvider(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public string GetPathForLoadFile(string filePath)
		{
			return MakeRelativePath(_exportSettings.FolderPath, filePath);
		}

		public static string MakeRelativePath(string fromPath, string toPath)
		{
			if (string.IsNullOrEmpty(fromPath))
			{
				throw new ArgumentNullException(nameof(fromPath));
			}
			if (string.IsNullOrEmpty(toPath))
			{
				throw new ArgumentNullException(nameof(toPath));
			}

			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);

			if (fromUri.Scheme != toUri.Scheme)
			{
				return toPath;
			} // path can't be made relative.

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
			{
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return Path.Combine(".", relativePath);
		}
	}
}