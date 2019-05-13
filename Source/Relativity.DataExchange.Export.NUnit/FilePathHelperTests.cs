// -----------------------------------------------------------------------------------------------------
// <copyright file="FilePathHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.Logging;

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

			// ACT
			string relativePath = instance.MakeRelativePath(root, path);

			// ASSERT
			Assert.That(relativePath, Is.EqualTo(result));
		}
	}
}