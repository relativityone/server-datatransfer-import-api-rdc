using System.Configuration;

namespace SQLDataComparer.Config
{
	public class CompareConfig : ConfigurationSection
	{
		[ConfigurationProperty("tables")]
		[ConfigurationCollection(typeof(TablesConfig))]
		public TablesConfig TablesConfig
		{
			get { return (TablesConfig)this["tables"]; }
		}
	}
}
