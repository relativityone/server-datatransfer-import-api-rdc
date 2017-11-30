using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	[TestFixture]
	public class RelativeFilePathProviderTests
	{
		[Test]
		public void ItShouldReturnRelativePath()
		{
			var exportSettings = new ExportFile(1)
			{
				FolderPath = "C:\\ABC\\"
			};

			string absolutePath = "C:\\ABC\\DEF\\GHI";

			var instance = new RelativeFilePathProvider(exportSettings);

			//ACT
			string relativePath = instance.GetPathForLoadFile(absolutePath);

			//ASSERT
			Assert.That(relativePath, Is.EqualTo("DEF\\GHI"));
		}
	}
}