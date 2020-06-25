using System.Xml.Serialization;

namespace SQLDataComparer.Runner.InputDto
{
	public class TestWorkspace
	{
		[XmlAttribute("TestName")]
		public string TestName { get; set; }

		[XmlAttribute("DatabaseName")]
		public string DatabaseName { get; set; }

		[XmlAttribute("ComparerConfigFilePath")]
		public string ComparerConfigFilePath { get; set; }
	}
}
