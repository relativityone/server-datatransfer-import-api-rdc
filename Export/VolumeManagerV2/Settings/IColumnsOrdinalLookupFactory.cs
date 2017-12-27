using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public interface IColumnsOrdinalLookupFactory
	{
		Dictionary<string, int> CreateOrdinalLookup(ExportFile exportSettings, string[] columnNamesInOrder);
	}
}