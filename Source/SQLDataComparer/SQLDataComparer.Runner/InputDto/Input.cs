using System.Xml.Serialization;

namespace SQLDataComparer.Runner.InputDto
{
	[XmlRoot("TestWorkspaces")]
	public class Input
	{
		[XmlElement("TestWorkspace")]
		public TestWorkspace[] TestWorkspaces { get; set; }
	}
}
