using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface IImageLoadFile
	{
		void Create(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}