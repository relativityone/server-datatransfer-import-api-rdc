using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata
{
	[TestFixture]
	public class ErrorFileDestinationPathTests
	{
		[Test]
		public void ItShouldReturnSamePathEveryTime()
		{
			ExportFile exportSettings = new ExportFile(1);

			var instance = new ErrorFileDestinationPath(exportSettings);

			//ACT
			string path1 = instance.Path;
			string path2 = instance.Path;

			//ASSERT
			Assert.That(path1, Is.Not.Null.Or.Empty);
			Assert.That(path1, Is.EqualTo(path2));
		}
	}
}