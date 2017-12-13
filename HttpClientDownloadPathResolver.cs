using System;
using Relativity.Transfer;

namespace kCura.WinEDDS.TApi
{
	internal class HttpClientDownloadPathResolver : RemotePathResolverBase
	{
		protected override string OnResolvePath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException(nameof(path));
			}
			return FileSystemService.GetFileName(path);
		}
	}
}