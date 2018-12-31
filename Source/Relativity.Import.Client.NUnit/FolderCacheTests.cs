// ----------------------------------------------------------------------------
// <copyright file="FolderCacheTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exceptions;
	using kCura.WinEDDS.Helpers;
	using kCura.WinEDDS.Importers;
	using kCura.WinEDDS.Service;

	using Moq;

	/// <summary>
	/// Represents <see cref="FolderCache"/> tests.
	/// </summary>
	[TestFixture]
    public class FolderCacheTests
	{
		private const int TestRootFolderId = 100000;
		private const int TestWorkspaceId = 2000000;

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1811:AvoidUncalledPrivateCode",
			Justification = "This is used by the test class.")]
		private static IEnumerable<TestCaseData> CreateFolderTestCases
		{
			// ReSharper disable once UnusedMember.Local
			get
			{
				for (var i = 0; i < 2; i++)
				{
					bool clearRows = i != 0;
					yield return new TestCaseData("\\", 1, clearRows);
					yield return new TestCaseData("\\1", 2, clearRows);
					yield return new TestCaseData("\\1\\1", 3, clearRows);
					yield return new TestCaseData("\\1\\1\\1", 4, clearRows);
					yield return new TestCaseData("\\1\\1\\2", 4, clearRows);
					yield return new TestCaseData("\\1\\2", 3, clearRows);
					yield return new TestCaseData("\\1\\2\\1", 4, clearRows);
					yield return new TestCaseData("\\1\\2\\2", 4, clearRows);
					yield return new TestCaseData("\\2", 2, clearRows);
					yield return new TestCaseData("\\2\\1", 3, clearRows);
					yield return new TestCaseData("\\2\\1\\1", 4, clearRows);
					yield return new TestCaseData("\\2\\1\\2", 4, clearRows);
					yield return new TestCaseData("\\2\\2", 3, clearRows);
					yield return new TestCaseData("\\2\\2\\1", 4, clearRows);
					yield return new TestCaseData("\\2\\2\\2", 4, clearRows);
				}
			}
		}

		private DataSet mockDataSet;
		private DataTable mockDataTable;
		private Mock<IHierarchicArtifactManager> mockFolderManager;
		private Mock<Logging.ILog> mockLogger;

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "The test class handles the disposal.")]
		public void SetUp()
		{
			// This allows the same dataset to be used with multiple tests.
			this.mockDataSet = new DataSet("mock-dataset") { Locale = CultureInfo.CurrentCulture };
			this.mockDataTable = new DataTable("mock-datatable") { Locale = CultureInfo.CurrentCulture };
			this.mockDataTable.Columns.Add("ArtifactID", typeof(int));
			this.mockDataTable.Columns.Add("ParentArtifactID", typeof(int));
			this.mockDataTable.Columns.Add("Name", typeof(string));
			this.mockDataSet.Tables.Add(this.mockDataTable);
			this.mockFolderManager = new Mock<IHierarchicArtifactManager>();
			this.mockFolderManager.Setup(x => x.RetrieveArtifacts(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.mockDataSet);
			this.mockFolderManager.Setup(x => x.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.Returns(RandomHelper.NextInt32(2000000, 9000000));
			this.mockFolderManager.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.Returns(FolderCache.FolderNotFoundId);
			this.mockLogger = new Mock<Logging.ILog>();
		}

		/// <summary>
		/// The test teardown.
		/// </summary>
		[TearDown]
		public void Teardown()
		{
			this.mockDataSet.Dispose();
			this.mockDataTable.Dispose();
		}

		/// <summary>
		/// An exception is thrown when the <see cref="Logging.ILog"/> is null.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		public void ShouldThrowWhenTheLoggerIsNull()
		{
			// Arrange
			this.mockDataSet.Clear();

			// Act
			ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
				() =>
				{
					IFolderCache folderCache = new FolderCache(null, this.mockFolderManager.Object, TestRootFolderId,
						TestWorkspaceId);
					Assert.That(folderCache, Is.Not.Null);
				});

			// Assert
			Console.WriteLine(exception.Message);
		}

		/// <summary>
		/// An exception is thrown when the <see cref="IHierarchicArtifactManager"/> is null.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		public void ShouldThrowWhenTheHierarchicalArtifactManagerIsNull()
		{
			// Arrange
			this.mockDataSet.Clear();

			// Act
			ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
				() =>
				{
					IFolderCache folderCache =
						new FolderCache(this.mockLogger.Object, null, TestRootFolderId, TestWorkspaceId);
					Assert.That(folderCache, Is.Not.Null);
				});

			// Assert
			Console.WriteLine(exception.Message);
		}

		/// <summary>
		/// An exception is thrown when the root folder artifact identifier is out of range.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(-1000)]
		public void ShouldThrowWhenTheRootFolderIdIsOutOfRange(int rootFolderId)
		{
			// Arrange
			this.mockDataSet.Clear();

			// Act
			ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
				() =>
				{
					IFolderCache folderCache = new FolderCache(
						this.mockLogger.Object,
						this.mockFolderManager.Object,
						rootFolderId,
						TestWorkspaceId);
					Assert.That(folderCache, Is.Not.Null);
				});

			// Assert
			Console.WriteLine(exception.Message);
		}

		/// <summary>Verifies that all well-known fatal exceptions are rethrown.</summary>
		/// <param name="create">
		/// Specify whether creating a folder should throw an exception.
		/// </param>
		/// <param name="read">
		/// Specify whether reading a folder should throw an exception.
		/// </param>
		/// <param name="retrieve">
		/// Specify whether retrieving all folders should throw an exception.
		/// </param>
		[Test]
		[TestCase(true, false, false)]
		[TestCase(false, true, false)]
		[TestCase(false, false, true)]
		[Category(TransferTestCategories.UnitTest)]
		public void ShouldRethrowFatalExceptions(bool create, bool read, bool retrieve)
		{
			int skipped = 0;
			foreach (Type fatalExceptionType in ExceptionHelper.FatalExceptionCandidates)
			{
				// Arrange
				this.mockDataTable.Rows.Clear();
				this.mockDataTable.Constraints.Clear();
				this.mockDataSet.Relations.Clear();
				this.mockFolderManager.Invocations.Clear();
				Exception fatalException;

				try
				{
					fatalException = Activator.CreateInstance(fatalExceptionType) as Exception;
				}
				catch (MissingMethodException)
				{
					// Skip parameter-less exception types.
					skipped++;
					continue;
				}

				if (create)
				{
					this.mockFolderManager.Setup(x => x.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
						.Throws(fatalException);
				}
				else if (read)
				{
					this.mockFolderManager.Setup(x => x.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
						.Throws(fatalException);
				}
				else if (retrieve)
				{
					this.mockFolderManager.Setup(x => x.RetrieveArtifacts(It.IsAny<int>(), It.IsAny<int>()))
						.Throws(fatalException);
				}

				// Act
				Exception exception = Assert.Throws(
					fatalExceptionType,
					() =>
						{
							IFolderCache folderCache = new FolderCache(
								this.mockLogger.Object,
								this.mockFolderManager.Object,
								TestRootFolderId,
								TestWorkspaceId);
							folderCache.GetFolderId("\\abc\\def");
						});

				// Assert
				Assert.That(exception, Is.TypeOf(fatalExceptionType));
				this.mockLogger.Verify(
					x => x.LogFatal(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
					Times.Never);
			}

			// We want to fail if any expectations change.
			Assert.That(skipped, Is.EqualTo(1));
		}

		/// <summary>
		/// Verifies that server-side read folder errors throw <see cref="WebApiException"/>.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		public void ShouldThrowWhenReadingFoldersFails()
		{
			// Arrange
			this.mockFolderManager.Invocations.Clear();
			this.mockDataTable.Rows.Clear();
			this.mockFolderManager.Setup(x => x.RetrieveArtifacts(It.IsAny<int>(), It.IsAny<int>()))
				.Throws(new SoapException());

			// Act
			WebApiException exception = Assert.Throws<WebApiException>(
				() =>
				{
					IFolderCache folderCache = new FolderCache(
						this.mockLogger.Object,
						this.mockFolderManager.Object,
						TestRootFolderId,
						TestWorkspaceId);
					Assert.That(folderCache, Is.Not.Null);
				});

			// Assert
			Assert.That(exception.Message, Does.EndWith(ExceptionHelper.TryAgainAdminFatalMessage));
			this.mockLogger.Verify(
				x => x.LogFatal(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Once);
			Console.WriteLine(exception.Message);
		}

		/// <summary>
		/// Verifies that server-side create folder errors throw <see cref="WebApiException"/>.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		public void ShouldThrowWhenCreatingFoldersFails()
		{
			// Arrange
			this.mockFolderManager.Invocations.Clear();
			this.mockDataTable.Rows.Clear();
			this.mockFolderManager.Setup(x => x.Create(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.Throws(new SoapException());
			IFolderCache cache = new FolderCache(
				this.mockLogger.Object,
				this.mockFolderManager.Object,
				TestRootFolderId,
				TestWorkspaceId);

			// Act
			WebApiException exception = Assert.Throws<WebApiException>(() =>
			{
				cache.GetFolderId("\\abc");
			});

			// Assert
			Assert.That(exception.Message, Does.EndWith(ExceptionHelper.TryAgainAdminFatalMessage));
			this.mockLogger.Verify(
				x => x.LogFatal(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Once);
			Console.WriteLine(exception.Message);
		}

		/// <summary>
		/// An exception is thrown when the workspace artifact identifier is out of range.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(-1000)]
		public void ShouldThrowWhenTheWorkspaceIdIsOutOfRange(int workspaceId)
		{
			// Arrange
			this.mockDataSet.Clear();

			// Act
			ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
				() =>
					{
						IFolderCache folderCache = new FolderCache(
							this.mockLogger.Object,
							this.mockFolderManager.Object,
							TestRootFolderId,
							workspaceId);
						Assert.That(folderCache, Is.Not.Null);
					});

			// Assert
			Console.WriteLine(exception.Message);
		}

		/// <summary>
		/// Verifies that non-existent folders are created using the mock folder manager.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		[TestCaseSource(nameof(CreateFolderTestCases))]
		public void ShouldCreateTheFolder(string folderPath, int expectedCacheCount, bool clearRows)
		{
			// Arrange
			this.mockFolderManager.Invocations.Clear();
			if (clearRows)
			{
				this.mockDataTable.Rows.Clear();
			}

			IFolderCache cache = new FolderCache(
				this.mockLogger.Object,
				this.mockFolderManager.Object,
				TestRootFolderId,
				TestWorkspaceId);

			// Act
			int artifactId = cache.GetFolderId(folderPath);

			// Assert
			Times expectedTimes;
			if (string.Compare(folderPath, "\\", StringComparison.OrdinalIgnoreCase) == 0)
			{
				expectedTimes = Times.Never();
				Assert.That(artifactId, Is.EqualTo(TestRootFolderId));
			}
			else
			{
				expectedTimes = Times.AtLeastOnce();
				Assert.That(artifactId, Is.GreaterThanOrEqualTo(2000000).And.LessThanOrEqualTo(9000000));
			}

			Assert.That(cache.Count, Is.EqualTo(expectedCacheCount));
			this.mockFolderManager.Verify(
				manager => manager.Create(TestWorkspaceId, It.IsAny<int>(), It.IsAny<string>()), expectedTimes);
			this.mockFolderManager.Verify(
				manager => manager.Read(TestWorkspaceId, It.IsAny<int>(), It.IsAny<string>()), expectedTimes);
		}

		/// <summary>
		/// Verifies that each specified path returns the expected folder artifact identifier from the mocked dataset.
		/// </summary>
		[Test]
		[Category(TransferTestCategories.UnitTest)]
		[TestCase("\\", 100000)]
		[TestCase("\\1", 100001)]
		[TestCase("\\1\\2", 100002)]
		public void ShouldReturnTheExistingFolderId(string folderPath, int expectedArtifactId)
		{
			// Arrange
			this.mockDataTable.Rows.Clear();
			this.mockDataTable.Rows.Add(TestRootFolderId, DBNull.Value, "\\");
			this.mockDataTable.Rows.Add(TestRootFolderId + 1, TestRootFolderId, "1");
			this.mockDataTable.Rows.Add(TestRootFolderId + 2, TestRootFolderId + 1, "2");
			IFolderCache cache = new FolderCache(
				this.mockLogger.Object,
				this.mockFolderManager.Object,
				100000,
				TestWorkspaceId);
			this.mockFolderManager.Invocations.Clear();

			// This will ensure the total number of items in the cache never increases.
			for (int i = 0; i < 3; i++)
			{
				// Act
				int artifactId = cache.GetFolderId(folderPath);

				// Assert
				Assert.That(artifactId, Is.EqualTo(expectedArtifactId));
				Assert.That(cache.Count, Is.EqualTo(this.mockDataTable.Rows.Count));
				Times expectedTimes = Times.Never();
				this.mockFolderManager.Verify(
					manager => manager.Create(TestWorkspaceId, It.IsAny<int>(), It.IsAny<string>()), expectedTimes);
				this.mockFolderManager.Verify(
					manager => manager.Read(TestWorkspaceId, It.IsAny<int>(), It.IsAny<string>()), expectedTimes);
			}
		}
	}
}