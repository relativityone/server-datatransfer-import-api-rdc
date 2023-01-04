using System.Xml.Serialization;

namespace SQLDataComparer.Config
{
	public class IgnoreConfig 
	{
		[XmlAttribute("name")]
		public string Name { get; set; }
	}
}
