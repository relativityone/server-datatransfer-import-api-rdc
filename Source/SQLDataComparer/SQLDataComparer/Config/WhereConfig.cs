using System.Xml.Serialization;

namespace SQLDataComparer.Config
{
	public enum WhereType
	{
		Is,
		IsNot,
	}

	public class WhereConfig
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("type")]
		public WhereType Type { get; set; }
		
		[XmlAttribute("value")]
		public string Value { get; set; }
	}
}
