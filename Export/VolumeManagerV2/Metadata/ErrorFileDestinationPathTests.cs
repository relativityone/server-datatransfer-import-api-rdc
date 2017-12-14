using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata
{
	[TestFixture]
	public class ErrorFileDestinationPathTests
	{
		[Test]
		public void ItShouldReturnSamePathEveryTime()
		{
			ExportFile exportSettings = new ExportFile(1);

			var instance = new ErrorFileDestinationPath(exportSettings, new NullLogger());

			//ACT
			string path1 = instance.Path;
			string path2 = instance.Path;

			//ASSERT
			Assert.That(path1, Is.Not.Null.Or.Empty);
			Assert.That(path1, Is.EqualTo(path2));
		}
	}
}