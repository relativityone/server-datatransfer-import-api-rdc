using System;
using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public interface IFullTextLoadFileEntry : IDisposable
	{
		bool TryCreateFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, out KeyValuePair<string, string> fullTextEntry);
	}
}