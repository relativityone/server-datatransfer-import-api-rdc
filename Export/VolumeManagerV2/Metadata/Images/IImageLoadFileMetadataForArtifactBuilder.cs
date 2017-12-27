using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface IImageLoadFileMetadataForArtifactBuilder
	{
		void WriteLoadFileEntry(ObjectExportInfo artifact, IRetryableStreamWriter writer, CancellationToken cancellationToken);
	}
}