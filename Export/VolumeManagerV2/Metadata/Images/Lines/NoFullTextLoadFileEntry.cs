using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class NoFullTextLoadFileEntry : IFullTextLoadFileEntry
	{
		public bool TryCreateFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, out KeyValuePair<string, string> fullTextEntry)
		{
			fullTextEntry = new KeyValuePair<string, string>();
			return false;
		}

		public void Dispose()
		{
		}
	}
}