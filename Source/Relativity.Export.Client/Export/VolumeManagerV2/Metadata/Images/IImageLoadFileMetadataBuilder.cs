using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface IImageLoadFileMetadataBuilder
	{
		void CreateLoadFileEntries(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}