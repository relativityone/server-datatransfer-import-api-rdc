using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface IImageLoadFileMetadataBuilder
	{
		IList<KeyValuePair<string, string>> CreateLoadFileEntries(ObjectExportInfo[] artifacts, CancellationToken cancellationToken);
	}
}