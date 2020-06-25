using System.Xml.Serialization;

namespace SQLDataComparer.Config
{
	[XmlRoot("compareConfig")]
	public class CompareConfig
	{
		[XmlArray("tables")]
		[XmlArrayItem("table")]
		public TableConfig[] TablesConfig { get; set; }

		public CompareConfig()
		{
			TablesConfig = new TableConfig[0];
		}
	}
}
