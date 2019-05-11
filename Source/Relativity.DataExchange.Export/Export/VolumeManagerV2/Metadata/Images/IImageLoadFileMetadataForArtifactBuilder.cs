﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;

	public interface IImageLoadFileMetadataForArtifactBuilder
	{
		void WriteLoadFileEntry(ObjectExportInfo artifact, IRetryableStreamWriter writer, CancellationToken cancellationToken);
	}
}