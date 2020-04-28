using System.Configuration;

namespace SQLDataComparer.Config
{
	public enum MappingType
	{
		SingleObject,
		SingleChoice,
		MultiChoice,
		MultiObject
	}

	public class MappingConfig : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public MappingType Type
		{
			get { return (MappingType)base["type"]; }
			set { base["type"] = value; }
		}

		[ConfigurationProperty("targetTable", IsRequired = true)]
		public string TargetTable
		{
			get { return (string)base["targetTable"]; }
			set { base["targetTable"] = value; }
		}
	}
}
