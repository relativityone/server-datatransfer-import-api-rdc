using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class EmptyImageLoadFileMetadataBuilder : IImageLoadFileMetadataBuilder
	{
		public IList<KeyValuePair<string, string>> CreateLoadFileEntries(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			return new List<KeyValuePair<string, string>>();
		}
	}
}