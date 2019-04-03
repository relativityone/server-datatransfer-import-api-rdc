﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="OutsideInFileIdServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="FileIDService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using global::NUnit.Framework;

	using Relativity.Import.Export;
	using Relativity.Import.Export.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="OutsideInFileIdService"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class OutsideInFileIdServiceTests
	{
        private IFileIdService service;
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
			this.service = new OutsideInFileIdService();
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

		[IdentifiedTest("04cba75c-0b5e-4c71-9b8f-cee069567be9")]
		[Category(TestCategories.OutsideIn)]
		[Category(TestCategories.Integration)]
		public void ItShouldIdentifyTheFile()
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

		[IdentifiedTest("9ee06912-e5e8-4377-8122-67ba8e086d59")]
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

		[IdentifiedTest("eb958881-5ba6-429a-b2b9-eba632cf5d50")]
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
				FileIdException exception = Assert.Throws<FileIdException>(() => this.service.Identify(this.tempFile));
				Assert.That(exception.Error, Is.EqualTo(FileIdError.Permissions));
				this.ValidateConfigInfo();
			}
		}

		[IdentifiedTest("01923012-4e9e-422a-a59c-fd3cbf38c373")]
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
			var configuration = this.service.Configuration;
			Assert.That(configuration, Is.Not.Null);
			Assert.That(configuration.InstallDirectory, Does.Exist);
			Assert.That(configuration.Version, Is.Not.Null.Or.Empty);
			Assert.That(configuration.Exception, Is.Null);
			Assert.That(configuration.HasError, Is.False);
		}
	}
}