using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface IImageLoadFileMetadataBuilder
	{
		IList<KeyValuePair<string, string>> CreateLoadFileEntries(ObjectExportInfo[] artifacts);
	}
}