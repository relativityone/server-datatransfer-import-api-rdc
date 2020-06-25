using System.Xml.Serialization;

namespace SQLDataComparer.Config
{
	public class WhereConfig
	{
		[XmlAttribute("name")]
		public string Name { get; set; }
		
		[XmlAttribute("value")]
		public string Value { get; set; }
	}
}
