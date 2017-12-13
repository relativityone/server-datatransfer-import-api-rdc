using System;
using System.IO;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class FilePathHelper
	{
		private readonly ILog _logger;

		public FilePathHelper(ILog logger)
		{
			_logger = logger;
		}

		public string MakeRelativePath(string fromPath, string toPath)
		{
			_logger.LogVerbose("Trying to make path {toPath} relative to path {fromPath}.", toPath, fromPath);
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
				_logger.LogWarning("Path cannot be made relative.");
				return toPath;
			} // path can't be made relative.

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
			{
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			_logger.LogVerbose("Relative path result {relativePath}.", relativePath);
			return relativePath;
		}
	}
}