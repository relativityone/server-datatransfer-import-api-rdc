using System.Xml.Serialization;

namespace SQLDataComparer.Config
{
	public enum MappingType
	{
		SingleObject,
		SingleChoice,
		MultiChoice,
		MultiObject,
		Artifact
	}

	public class MappingConfig
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("type")]
		public MappingType Type { get; set; }

		[XmlAttribute("targetTable")]
		public string TargetTable { get; set; }

		[XmlAttribute("targetColumn")] 
		public string TargetColumn { get; set; } = "ArtifactID";
	}
}
