﻿namespace Relativity.Export.VolumeManagerV2.Download
{
	using System.Threading;

	public interface IDownloader
	{
		void DownloadFilesForArtifacts(CancellationToken cancellationToken);
	}
}