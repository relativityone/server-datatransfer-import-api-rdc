namespace Relativity.Export.VolumeManagerV2.Settings
{
	using System.Collections.Generic;

	using kCura.WinEDDS;

	public interface IColumnsOrdinalLookupFactory
	{
		Dictionary<string, int> CreateOrdinalLookup(ExportFile exportSettings, string[] columnNamesInOrder);
	}
}