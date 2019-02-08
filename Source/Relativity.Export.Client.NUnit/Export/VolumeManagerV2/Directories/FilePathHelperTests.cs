using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	[TestFixture]
	public class FilePathHelperTests
	{
		[Test]
		[TestCase(@"C:\ABC\", @"C:\ABC\DEF\GHI", @"DEF\GHI")]
		[TestCase(@"\\NETWORK\", @"\\NETWORK\DEF\GHI", @"DEF\GHI")]
		[TestCase(@"X:\ROOT", @"X:\ROOT\X1\X2\", @"ROOT\X1\X2\", Description = "Missing slash")]
		public void ItShouldReturnRelativePath(string root, string path, string result)
		{
			var instance = new FilePathHelper(new NullLogger());

			//ACT
			string relativePath = instance.MakeRelativePath(root, path);

			//ASSERT
			Assert.That(relativePath, Is.EqualTo(result));
		}
	}
}