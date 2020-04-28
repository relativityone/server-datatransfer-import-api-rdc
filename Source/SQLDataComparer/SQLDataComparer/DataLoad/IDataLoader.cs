using System.Collections.Generic;
using SQLDataComparer.Config;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataLoad
{
	public interface IDataLoader
	{
		IEnumerable<Table> GetDataTable(TableConfig tableConfig, SideEnum side);
		IEnumerable<Table> GetMappingTable(TableConfig tableConfig, MappingConfig mappingConfig, Dictionary<string, string> mappingTable, SideEnum side);
		IEnumerable<Table> GetAuditTable(TableConfig auditConfig, Dictionary<string, string> mappingTable, SideEnum side);
	}
}
