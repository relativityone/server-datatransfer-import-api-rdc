using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface ILoadFileEntry
	{
		KeyValuePair<string, string> Create(string batesNumber, string filePath, int pageNumber, long pageOffset, int numberOfImages);
	}
}