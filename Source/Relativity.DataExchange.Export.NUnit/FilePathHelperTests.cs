// -----------------------------------------------------------------------------------------------------
// <copyright file="FilePathHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.TestFramework;
    using System;

	[TestFixture]
	public class FilePathHelperTests
	{
		[Test]
		[TestCase(@"C:\ABC\", @"C:\ABC\DEF\GHI", @"DEF\GHI")]
		[TestCase(@"\\NETWORK\", @"\\NETWORK\DEF\GHI", @"DEF\GHI")]
		[TestCase(@"X:\ROOT", @"X:\ROOT\X1\X2\", @"ROOT\X1\X2\", Description = "Missing slash")]
		public void ItShouldReturnRelativePath(string root, string path, string result)
		{
			var instance = new FilePathHelper(new TestNullLogger());

			// ACT
			string relativePath = instance.MakeRelativePath(root, path);

			// ASSERT
			Assert.That(relativePath, Is.EqualTo(result));
		}
        [Test]
        public void MakeRelativePath_ThrowsArgumentNullException_WhenFromPathIsNull()
        {
            // Arrange
            var instance = new FilePathHelper(new TestNullLogger());
            string fromPath = null;
            string toPath = "C:\\test";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => instance.MakeRelativePath(fromPath, toPath));
        }

        [Test]
        public void MakeRelativePath_ThrowsArgumentNullException_WhenFromPathIsEmpty()
        {
            // Arrange
            var instance = new FilePathHelper(new TestNullLogger());
            string fromPath = string.Empty;
            string toPath = "C:\\test";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => instance.MakeRelativePath(fromPath, toPath));
        }
        [Test]
        public void MakeRelativePath_ThrowsArgumentNull_WhenToPathIsNull()
        {
            // Arrange
            var instance = new FilePathHelper(new TestNullLogger());
            string fromPath = "C:\\test";
            string toPath = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => instance.MakeRelativePath(fromPath, toPath));
        }
        [Test]
        public void MakeRelativePath_ThrowsArgumentNull_WhenToPathIsEmpty()
        {
            // Arrange
            var instance = new FilePathHelper(new TestNullLogger());
            string fromPath = "C:\\test";
            string toPath = string.Empty;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => instance.MakeRelativePath(fromPath, toPath));
        }

        [Test]
        public void MakeRelativePath_ReturnsToPath_WhenSchemesDiffer()
        {
            // ...

            var instance = new FilePathHelper(new TestNullLogger());
            string fromPath = "http://example.com";
            string toPath = "file://C:/test";

            // Act
            string relativePath = instance.MakeRelativePath(fromPath, toPath);

            // Assert
            Assert.That(relativePath, Is.EqualTo(toPath));
        }
    }
}