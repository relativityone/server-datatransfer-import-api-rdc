// -----------------------------------------------------------------------------------------------------
// <copyright file="OutsideInFileTypeIdentifierServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="FileIDService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents <see cref="OutsideInFileTypeIdentifierService"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class OutsideInFileTypeIdentifierServiceTests
	{
		private static bool dumpBinaries;
		private TempDirectory2 tempDirectory;
		private string tempFile;

		[SetUp]
		public void Setup()
		{
			this.tempDirectory = new TempDirectory2();
			this.tempDirectory.Create();
			this.tempFile = System.IO.Path.Combine(
				this.tempDirectory.Directory,
				System.Guid.NewGuid().ToString("D").ToUpperInvariant());
			System.IO.File.WriteAllText(this.tempFile, "Hello World");

			try
			{
				OutsideInFileTypeIdentifierService.Shutdown();
			}
			catch
			{
				DumpBinaries();
				throw;
			}
		}

		[TearDown]
		public void Teardown()
		{
			this.tempDirectory.Dispose();
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		public void ShouldIdentifyTheFile()
		{
			try
			{
				for (int i = 0; i < 2; i++)
				{
					// Reinitializing should have no impact.
					FileTypeIdentifierService.Instance.Reinitialize();
					IFileTypeInfo fileTypeInfo = FileTypeIdentifierService.Instance.Identify(this.tempFile);
					Assert.That(fileTypeInfo, Is.Not.Null);
					Assert.That(fileTypeInfo.Id, Is.GreaterThan(0));
					Assert.That(fileTypeInfo.Description, Is.Not.Null.Or.Empty);
				}

				this.ValidateConfigInfo();
			}
			catch
			{
				DumpBinaries();
				throw;
			}
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[Category(TestCategories.OutsideIn)]
		public void ShouldThrowWhenTheFileIsNullOrEmpty(string file)
		{
			try
			{
				Assert.Throws<System.ArgumentNullException>(() => FileTypeIdentifierService.Instance.Identify(file));
			}
			catch
			{
				DumpBinaries();
				throw;
			}
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		public void ShouldThrowWhenTheFileDoesNotExist()
		{
			try
			{
				string fileName = System.Guid.NewGuid().ToString("D").ToUpperInvariant();
				string invalidFile = System.IO.Path.Combine(this.tempDirectory.Directory, fileName);
				Assert.Throws<System.IO.FileNotFoundException>(() => FileTypeIdentifierService.Instance.Identify(invalidFile));
				this.ValidateConfigInfo();
			}
			catch
			{
				DumpBinaries();
				throw;
			}
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		public void ShouldThrowWhenTheFileIsLocked()
		{
			using (System.IO.File.Open(
					this.tempFile,
					System.IO.FileMode.Open,
					System.IO.FileAccess.Read,
					System.IO.FileShare.None))
			{
				try
				{
					FileTypeIdentifyException exception =
						Assert.Throws<FileTypeIdentifyException>(() => FileTypeIdentifierService.Instance.Identify(this.tempFile));
					Assert.That(exception.Error, Is.EqualTo(FileTypeIdentifyError.Permissions));
					this.ValidateConfigInfo();
				}
				catch
				{
					DumpBinaries();
					throw;
				}
			}
		}

		[Test]
		[Category(TestCategories.OutsideIn)]
		public void ShouldGetTheConfigInfo()
		{
			for (int i = 0; i < 5; i++)
			{
				try
				{
					this.ValidateConfigInfo();
					FileTypeIdentifierService.Instance.Identify(this.tempFile);
					this.ValidateConfigInfo();
					FileTypeIdentifierService.Instance.Reinitialize();
					this.ValidateConfigInfo();
				}
				catch
				{
					DumpBinaries();
					throw;
				}
			}
		}

		private static void DumpBinaries()
		{
			if (dumpBinaries)
			{
				return;
			}

			try
			{
				string path = OutsideInFileTypeIdentifierService.GetInstallPath();
				Console.WriteLine(
					$"A serious OI error has occurred. Dumping the list of OI binaries found within the '{path}' install directory.");
				if (!System.IO.Directory.Exists(path))
				{
					Console.WriteLine($"The OI '{path}' install directory doesn't exist.");
					Console.WriteLine(
						"Check the project file and custom OI target to ensure the OI binaries are properly copied to the target path.");
				}
				else
				{
					List<string> files = System.IO.Directory.GetFiles(path).ToList();
					if (files.Count > 0)
					{
						foreach (string file in files)
						{
							Console.WriteLine($"OI binary: {file}");
						}
					}
					else
					{
						Console.WriteLine($"No OI binaries were found within the '{path}' install directory.");
						Console.WriteLine("Check the build scripts and logs for build warnings.");
					}
				}
			}
			finally
			{
				dumpBinaries = true;
			}
		}

		private void ValidateConfigInfo()
		{
			var configuration = FileTypeIdentifierService.Instance.Configuration;
			Assert.That(configuration, Is.Not.Null);
			Assert.That(configuration.Exception, Is.Null);
			Assert.That(configuration.InstallDirectory, Does.Exist);
			Assert.That(configuration.Version, Is.Not.Null.Or.Empty);
			Assert.That(configuration.HasError, Is.False);
		}
	}
}