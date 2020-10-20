using System.Xml.Serialization;

namespace SQLDataComparer.Config
{
	public class MappedStringConfig
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("left")]
		public string Left { get; set; }

		[XmlAttribute("right")]
		public string Right { get; set; }
	}
}
