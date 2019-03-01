// -----------------------------------------------------------------------------------------------------
// <copyright file="OutsideInFileIdServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="FileIDService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit.Integration
{
	using global::NUnit.Framework;

	using Relativity.Import.Export;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="OutsideInFileIdService"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class OutsideInFileIdServiceTests
	{
		private const int Timeout = 35;
		private IFileIdService service;
		private TempDirectory tempDirectory;
		private string tempFile;

		[SetUp]
		public void Setup()
		{
			this.tempDirectory = new TempDirectory();
			this.tempDirectory.Create();
			this.tempFile = System.IO.Path.Combine(
				this.tempDirectory.Directory,
				System.Guid.NewGuid().ToString("D").ToUpperInvariant());
			System.IO.File.WriteAllText(this.tempFile, "Hello World");
			this.service = new OutsideInFileIdService(Timeout);
		}

		[TearDown]
		public void Teardown()
		{
			this.tempDirectory.Dispose();
			if (this.service != null)
			{
				this.service.Dispose();
				this.service = null;
			}
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ShouldIdentifyTheFile()
		{
			for (int i = 0; i < 2; i++)
			{
				// Reinitializing should have no impact.
				this.service.Reinitialize();
				FileIdInfo fileIdData = this.service.Identify(this.tempFile);
				Assert.That(fileIdData, Is.Not.Null);
				Assert.That(fileIdData.Id, Is.GreaterThan(0));
				Assert.That(fileIdData.Description, Is.Not.Null.Or.Empty);
			}

			this.ValidateConfigInfo();
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ShouldThrowWhenTheFileIsNullOrEmpty(string file)
		{
			Assert.Throws<System.ArgumentNullException>(() => this.service.Identify(file));
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ShouldThrowWhenTheFileDoesNotExist()
		{
			string fileName = System.Guid.NewGuid().ToString("D").ToUpperInvariant();
			string invalidFile = System.IO.Path.Combine(this.tempDirectory.Directory, fileName);
			Assert.Throws<System.IO.FileNotFoundException>(() => this.service.Identify(invalidFile));
			this.ValidateConfigInfo();
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ShouldThrowWhenTheFileIsLocked()
		{
			using (System.IO.File.Open(
					this.tempFile,
					System.IO.FileMode.Open,
					System.IO.FileAccess.Read,
					System.IO.FileShare.None))
			{
				FileIdException exception = Assert.Throws<FileIdException>(() => this.service.Identify(this.tempFile));
				Assert.That(exception.Error, Is.EqualTo(FileIdError.Permissions));
				this.ValidateConfigInfo();
			}
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ShouldGetTheConfigInfo()
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
			var configuration = this.service.Configuration;
			Assert.That(configuration, Is.Not.Null);
			Assert.That(configuration.InstallDirectory, Does.Exist);
			Assert.That(configuration.Version, Is.Not.Null.Or.Empty);
			Assert.That(configuration.Exception, Is.Null);
			Assert.That(configuration.HasError, Is.False);
			Assert.That(configuration.Timeout, Is.EqualTo(Timeout));
		}
	}
}