using System.Xml.Serialization;

namespace SQLDataComparer.Config
{
	public class TableConfig
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("rowId")]
		public string RowId { get; set; }

		[XmlAttribute("mapId")] 
		public string MapId { get; set; } = "ArtifactID";

		[XmlArray("mappings")]
		[XmlArrayItem("mapping")]
		public MappingConfig[] MappingsConfig { get; set; }

		[XmlArray("ignored")]
		[XmlArrayItem("ignore")]
		public IgnoreConfig[] IgnoreConfig { get; set; }

		[XmlArray("whereClauses")]
		[XmlArrayItem("where")]
		public WhereConfig[] WhereConfig { get; set; }

		[XmlArray("mappedStrings")]
		[XmlArrayItem("mappedString")]
		public MappedStringConfig[] MappedStringConfig { get; set; }

		public TableConfig()
		{
			MappingsConfig = new MappingConfig[0];
			IgnoreConfig = new IgnoreConfig[0];
			WhereConfig = new WhereConfig[0];
			MappedStringConfig = new MappedStringConfig[0];
		}
	}
}
