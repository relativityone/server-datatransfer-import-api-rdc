using System.Collections.Generic;
using System.Configuration;

namespace SQLDataComparer.Config
{
	public class TableConfig : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}

		[ConfigurationProperty("rowId", IsRequired = true)]
		public string RowId
		{
			get { return (string)base["rowId"]; }
			set { base["rowId"] = value; }
		}

		[ConfigurationProperty("mappings", IsRequired = false)]
		[ConfigurationCollection(typeof(MappingsConfig))]
		public MappingsConfig MappingsConfig
		{
			get { return (MappingsConfig)this["mappings"]; }
		}

		[ConfigurationProperty("ignore", IsRequired = false)]
		[ConfigurationCollection(typeof(IgnoreConfig))]
		public IgnoreConfig IgnoreConfig
		{
			get { return (IgnoreConfig)this["ignore"]; }
		}

		[ConfigurationProperty("where", IsRequired = false)]
		[ConfigurationCollection(typeof(WhereConfig))]
		public WhereConfig WhereConfig
		{
			get { return (WhereConfig)this["where"]; }
		}
	}
}
