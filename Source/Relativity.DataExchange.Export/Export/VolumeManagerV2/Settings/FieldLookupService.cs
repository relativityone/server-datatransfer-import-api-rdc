namespace Relativity.DataExchange.Export.VolumeManagerV2.Settings
{
	using System.Collections.Generic;

	using kCura.WinEDDS;

	public class FieldLookupService : IFieldLookupService
	{
		private readonly Dictionary<string, int> _ordinalLookup;

		public FieldLookupService(Dictionary<string, int> ordinalLookup)
		{
			_ordinalLookup = ordinalLookup;
		}

		public int GetOrdinalIndex(string fieldName)
		{
			return _ordinalLookup[fieldName];
		}

		public bool ContainsFieldName(string fieldName)
		{
			return _ordinalLookup.ContainsKey(fieldName);
		}
	}
}