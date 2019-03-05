// -----------------------------------------------------------------------------------------------------
// <copyright file="RelativeFilePathProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

    using Relativity.Logging;

    [TestFixture]
	public class RelativeFilePathProviderTests
	{
		[Test]
		[TestCase(@"C:\ABC\", @"C:\ABC\DEF\GHI", @".\DEF\GHI")]
		[TestCase(@"\\NETWORK\", @"\\NETWORK\DEF\GHI", @".\DEF\GHI")]
		public void ItShouldReturnRelativePath(string root, string path, string result)
		{
			var exportSettings = new ExportFile(1)
			{
				FolderPath = root
			};

			string absolutePath = path;

			var instance = new RelativeFilePathTransformer(exportSettings, new FilePathHelper(new NullLogger()));

			// ACT
			string relativePath = instance.TransformPath(absolutePath);

			// ASSERT
			Assert.That(relativePath, Is.EqualTo(result));
		}
	}
}