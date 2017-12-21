using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	[TestFixture]
	public class PrefixFilePathTransformerTests
	{
		[Test]
		[TestCase(@"C:\ABC\", @"C:\ABC\DEF\GHI", @"", @"DEF\GHI")]
		[TestCase(@"C:\ABC\", @"C:\ABC\DEF\GHI", @"PREF", @"PREF\DEF\GHI")]
		[TestCase(@"\\NETWORK\", @"\\NETWORK\DEF\GHI", @"PREFIX", @"PREFIX\DEF\GHI")]
		public void ItShouldReturnPathWithPrefix(string root, string path, string prefix, string result)
		{
			var exportSettings = new ExportFile(1)
			{
				FolderPath = root,
				FilePrefix = prefix
			};
			
			var instance = new PrefixFilePathTransformer(exportSettings, new FilePathHelper(new NullLogger()));

			//ACT
			string pathWithPrefix = instance.TransformPath(path);

			//ASSERT
			Assert.That(pathWithPrefix, Is.EqualTo(result));
		}
	}
}