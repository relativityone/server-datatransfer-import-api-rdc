using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface IImageLoadFileMetadataForArtifactBuilder
	{
		void CreateLoadFileEntry(ObjectExportInfo artifact, IList<KeyValuePair<string, string>> lines);
	}
}