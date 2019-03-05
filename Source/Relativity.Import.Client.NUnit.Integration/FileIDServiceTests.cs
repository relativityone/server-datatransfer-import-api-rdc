// -----------------------------------------------------------------------------------------------------
// <copyright file="FileIDServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="FileIDService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using global::NUnit.Framework;

	using kCura.OI.FileID;

	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="FileIDService"/> tests.
	/// </summary>
	[TestFixture]
	public class FileIDServiceTests
	{
        private FileIDService service;
		private string tempFile;

		[SetUp]
		public void Setup()
		{
			string tempDirectory = System.IO.Path.GetTempPath();
			string fileName = string.Join(
				"-",
				System.DateTime.Now.ToString("yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture),
				System.Guid.NewGuid().ToString("D").ToUpperInvariant());
			this.tempFile = System.IO.Path.Combine(tempDirectory, fileName);
			System.IO.File.WriteAllText(this.tempFile, "Hello World");
			this.service = new FileIDService();
		}

		[TearDown]
		public void Teardown()
		{
			if (!string.IsNullOrEmpty(this.tempFile) && System.IO.File.Exists(this.tempFile))
			{
				System.IO.File.Delete(this.tempFile);
				this.tempFile = null;
			}

			if (this.service != null)
			{
				this.service.Dispose();
				this.service = null;
			}
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ItShouldIdentifyTheFile()
		{
			for (int i = 0; i < 2; i++)
			{
				// Reinitializing should have no impact.
				this.service.Reinitialize();
				FileIDData fileIdData = this.service.Identify(this.tempFile);
				Assert.That(fileIdData, Is.Not.Null);
				Assert.That(fileIdData.FileID, Is.GreaterThan(0));
				Assert.That(fileIdData.FileType, Is.Not.Null.Or.Empty);
			}

			this.ValidateConfigInfo();
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ItShouldThrowWhenTheFileDoesNotExist()
		{
			string tempDirectory = System.IO.Path.GetTempPath();
			string fileName = System.Guid.NewGuid().ToString("D").ToUpperInvariant();
			string invalidFile = System.IO.Path.Combine(tempDirectory, fileName);
			Assert.Throws<System.IO.FileNotFoundException>(() => this.service.Identify(invalidFile));
			this.ValidateConfigInfo();
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ItShouldThrowWhenTheFileIsLocked()
		{
			using (System.IO.File.Open(
					this.tempFile,
					System.IO.FileMode.Open,
					System.IO.FileAccess.Read,
					System.IO.FileShare.None))
			{
				FileIDIdentificationException exception =
					Assert.Throws<FileIDIdentificationException>(() => this.service.Identify(this.tempFile));
				Assert.That(exception.ErrorCode, Is.EqualTo(FileIDService.FilePermissionErrorCode));
				this.ValidateConfigInfo();
			}
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ItShouldGetTheConfigInfo()
		{
			for (int i = 0; i < 5; i++)
			{
				this.ValidateConfigInfo();
				this.service.Identify(this.tempFile);
				this.ValidateConfigInfo();
				this.service.Reinitialize();
				this.ValidateConfigInfo();
			}
		}

		private void ValidateConfigInfo()
		{
			var configInfo = this.service.GetConfigInfo();
			Assert.That(configInfo, Is.Not.Null);
			Assert.That(configInfo.InstallLocation, Does.Exist);
			Assert.That(configInfo.Version, Is.Not.Null.Or.Empty);
			Assert.That(configInfo.Exception, Is.Null);
			Assert.That(configInfo.HasError, Is.False);
		}
	}
}