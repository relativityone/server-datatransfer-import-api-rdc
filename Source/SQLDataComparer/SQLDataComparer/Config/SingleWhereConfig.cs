using System.Configuration;

namespace SQLDataComparer.Config
{
	public class SingleWhereConfig : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}

		[ConfigurationProperty("value", IsRequired = true)]
		public string Value
		{
			get { return (string)base["value"]; }
			set { base["value"] = value; }
		}

	}
}
