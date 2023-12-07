// -----------------------------------------------------------------------------------------------------
// <copyright file="FileShareSettingsServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
    using Relativity.Transfer;

    [TestFixture]
	public class FileShareSettingsServiceTests
	{
		private Mock<ILog> _logger;
		private Mock<ITapiObjectService> _tapiObjectService;
		private Mock<IStatus> _status;
		private IFileShareSettingsService _instance;
		private ExportFile _exportFile;

		[SetUp]
		public void SetUp()
		{
			this._exportFile = new ExportFile(12);
			this._exportFile.CaseInfo = new CaseInfo();
			this._exportFile.CaseInfo.ArtifactID = RandomHelper.NextInt(1000, 100000);
			this._exportFile.Credential = new NetworkCredential();
			var testNullLogger = new TestNullLogger();
			_logger = testNullLogger.NullLoggerMock;
           this._status = new Mock<IStatus>();
			this._tapiObjectService = new Mock<ITapiObjectService>();
			this._instance = new FileShareSettingsService(
				this._status.Object,
				this._tapiObjectService.Object,
				testNullLogger,
				this._exportFile);
		}


        [Test]
		public void ItShouldThrowWhenFatalExceptionIsThrown()
		{
			// ARRANGE
			this._tapiObjectService.Setup(
				x => x.GetWorkspaceDefaultFileShareAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<ILog>(),
					It.IsAny<CancellationToken>())).Throws(new OutOfMemoryException());

			// ACT
			Assert.ThrowsAsync<OutOfMemoryException>(() => this._instance.ReadFileSharesAsync(CancellationToken.None));

			// ASSERT
			this._logger.Verify(
				x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Never);
		}

		[Test]
		public async Task ItShouldNotThrowWhenTheDefaultWorkspaceFileShareIsNullAsync()
		{
			// ARRANGE
			this._tapiObjectService.Setup(
				x => x.GetWorkspaceDefaultFileShareAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<ILog>(),
					It.IsAny<CancellationToken>())).Returns(Task.FromResult((RelativityFileShare)null));

			// ACT
			await this._instance.ReadFileSharesAsync(CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			this._logger.Verify(x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task ItShouldReturnNullWhenTheDefaultRepositoryIsInvalidAsync(bool cloudInstance)
		{
			// ARRANGE
			RelativityFileShare defaultFileShare = CreateRelativityFileShare(cloudInstance, 1000);
			this._tapiObjectService.Setup(
				x => x.GetWorkspaceDefaultFileShareAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<ILog>(),
					It.IsAny<CancellationToken>())).Returns(Task.FromResult(defaultFileShare));
			this._tapiObjectService
				.Setup(
					x => x.SearchFileStorageAsync(
						It.IsAny<TapiBridgeParameters2>(),
						It.IsAny<ILog>(),
						It.IsAny<CancellationToken>())).Returns(
					Task.FromResult(
						CreateMockTapiFileStorageSearchResults(
							cloudInstance,
							new RelativityFileShare[] { },
							new[] { defaultFileShare })));

			// ACT
			await this._instance.ReadFileSharesAsync(CancellationToken.None).ConfigureAwait(false);
			IRelativityFileShareSettings settings = this._instance.GetSettingsForFileShare(1, @"\\files");

			// ASSERT. Being overly anal to check all boundaries and catch the slightest code deviations.
			Assert.That(settings, Is.Null);
			this._logger.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(3));
		}

		[Test]
		[TestCase(false, 0, 0, 1, 1)]
		[TestCase(true, 0, 0, 1, 1)]
		[TestCase(false, 0, 1, 1, 2)]
		[TestCase(true, 0, 1, 1, 2)]
		[TestCase(false, 1, 0, 2, 0)]
		[TestCase(true, 1, 0, 2, 0)]
		[TestCase(false, 1, 1, 2, 1)]
		[TestCase(true, 1, 1, 2, 1)]
		[TestCase(false, 2, 2, 3, 2)]
		[TestCase(true, 2, 2, 3, 2)]
		public async Task ItShouldNotThrowOnAnyFileStorageSearchResultAsync(
			bool cloudInstance,
			int totalValid,
			int totalInvalid,
			int expectedInformationLogs,
			int expectedWarningLogs)
		{
			// ARRANGE
			this._tapiObjectService.Setup(
				x => x.GetWorkspaceDefaultFileShareAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<ILog>(),
					It.IsAny<CancellationToken>())).Returns(Task.FromResult(CreateRelativityFileShare(cloudInstance, 1000)));
			this._tapiObjectService
				.Setup(
					x => x.SearchFileStorageAsync(
						It.IsAny<TapiBridgeParameters2>(),
						It.IsAny<ILog>(),
						It.IsAny<CancellationToken>())).Returns(
					Task.FromResult(CreateMockTapiFileStorageSearchResults(cloudInstance, totalValid, totalInvalid)));
			this._status.Setup(x => x.WriteStatusLineWithoutDocCount(It.IsAny<EventType2>(), It.IsAny<string>(), It.IsAny<bool>()));

			// ACT
			await this._instance.ReadFileSharesAsync(CancellationToken.None).ConfigureAwait(false);

			// ASSERT. Being overly anal to check all boundaries and catch the slightest code deviations.
			this._logger.Verify(
				x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(expectedInformationLogs));
			this._logger.Verify(
				x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(expectedWarningLogs));
			this._status.Verify(x => x.WriteWarningWithoutDocCount(It.IsAny<string>()), Times.Exactly(expectedWarningLogs));
			this._status.Verify(x => x.WriteStatusLineWithoutDocCount(EventType2.Status, It.IsAny<string>(), true), Times.Exactly(2));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public async Task ItShouldLogAndReturnNullWhenTheDefaultFileShareIsNullAsync(bool readFileShares)
		{
			// ARRANGE
			this._tapiObjectService.Setup(
				x => x.GetWorkspaceDefaultFileShareAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<ILog>(),
					It.IsAny<CancellationToken>())).Returns(Task.FromResult((RelativityFileShare)null));

			// ACT
			if (readFileShares)
			{
				// Retrieving the settings below should never fail!
				await this._instance.ReadFileSharesAsync(CancellationToken.None).ConfigureAwait(false);
			}

			IRelativityFileShareSettings settings = this._instance.GetSettingsForFileShare(1, "abc");

			// ASSERT
			Assert.That(settings, Is.Null);
			this._logger.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		}

		[Test]
		[TestCase(false, @"\\files1.relativity.one\Files\EDDS1", 1)]
		[TestCase(true, @"\\files1.relativity.one\Files\EDDS1", 1)]
		[TestCase(false, @"\\files2.relativity.one\Files\EDDS2", 1)]
		[TestCase(true, @"\\files2.relativity.one\Files\EDDS2", 2)]
		[TestCase(false, @"\\files9999.relativity.one\Files\EDDS9", 1)]
		[TestCase(true, @"\\files9999.relativity.one\Files\EDDS9", 0)]
		public async Task ItShouldGetTheMappedFileShareSettingsAsync(
			bool cloudInstance,
			string path,
			int expectedFileShareArtifactId)
		{
			// ARRANGE
			RelativityFileShare fileShare1 = CreateRelativityFileShare(cloudInstance, 1, @"\\files1.relativity.one\Files");
			RelativityFileShare fileShare2 = CreateRelativityFileShare(cloudInstance, 2, @"\\files2.relativity.one\Files");
			this._tapiObjectService.Setup(
				x => x.GetWorkspaceDefaultFileShareAsync(
					It.IsAny<TapiBridgeParameters2>(),
					It.IsAny<ILog>(),
					It.IsAny<CancellationToken>())).Returns(Task.FromResult(fileShare1));
			this._tapiObjectService
				.Setup(
					x => x.SearchFileStorageAsync(
						It.IsAny<TapiBridgeParameters2>(),
						It.IsAny<ILog>(),
						It.IsAny<CancellationToken>())).Returns(
					Task.FromResult(
						CreateMockTapiFileStorageSearchResults(
							cloudInstance,
							new[] { fileShare1, fileShare2 },
							new RelativityFileShare[] { })));

			// ACT
			await this._instance.ReadFileSharesAsync(CancellationToken.None).ConfigureAwait(false);
			IRelativityFileShareSettings settings = this._instance.GetSettingsForFileShare(1, path);

			// ASSERT
			if (expectedFileShareArtifactId == 0)
			{
				Assert.That(settings, Is.Null);
				this._logger.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
			}
			else
			{
				Assert.That(settings, Is.Not.Null);
				Assert.That(settings.ArtifactId, Is.EqualTo(expectedFileShareArtifactId));
				this._logger.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
			}
		}

        [Test]
        public void ItShouldThrowArgumentExceptionWhenCaseInfoIsNull()
        {
            // ARRANGE
            ExportFile exportFile = new ExportFile(12);
            exportFile.Credential = new NetworkCredential();

            // ASSERTION
            Assert.Throws<ArgumentException>(() => new FileShareSettingsService(
                this._status.Object,
                this._tapiObjectService.Object,
                _logger.Object,
                exportFile));
        }

        [Test]
        public void ItShouldThrowArgumentExceptionWhenCredentialIsNull()
        {

            // ARRANGE
            ExportFile exportFile = new ExportFile(12);
            exportFile.CaseInfo = new CaseInfo();
            exportFile.CaseInfo.ArtifactID = RandomHelper.NextInt(1000, 100000);


            // ASSERTION
            Assert.Throws<ArgumentException>(() => new FileShareSettingsService(
                this._status.Object,
                this._tapiObjectService.Object,
                _logger.Object,
                exportFile));

        }

        private static ITapiFileStorageSearchResults CreateMockTapiFileStorageSearchResults(
			bool cloudInstance,
			int totalValid,
			int totalInvalid)
		{
			List<RelativityFileShare> validFileShares = new List<RelativityFileShare>();
			List<RelativityFileShare> invalidFileShares = new List<RelativityFileShare>();
			for (int i = 0; i < totalValid; i++)
			{
				validFileShares.Add(CreateRelativityFileShare(cloudInstance, 1000 + i));
			}

			for (int i = 0; i < totalInvalid; i++)
			{
				invalidFileShares.Add(CreateRelativityFileShare(cloudInstance, 2000 + i));
			}

			return CreateMockTapiFileStorageSearchResults(cloudInstance, validFileShares, invalidFileShares);
		}

		private static ITapiFileStorageSearchResults CreateMockTapiFileStorageSearchResults(
			bool cloudInstance,
			IEnumerable<RelativityFileShare> validFileShares,
			IEnumerable<RelativityFileShare> invalidFileShares)
		{
			Mock<ITapiFileStorageSearchResults> results = new Mock<ITapiFileStorageSearchResults>();
			results.SetupGet(x => x.CloudInstance).Returns(cloudInstance);
			results.SetupGet(x => x.FileShares).Returns(validFileShares.ToList());
			results.SetupGet(x => x.InvalidFileShares).Returns(invalidFileShares.ToList());
			return results.Object;
		}

		private static RelativityFileShare CreateRelativityFileShare(bool cloudInstance, int artifactId)
		{
			return CreateRelativityFileShare(cloudInstance, artifactId, @"\\files" + artifactId);
		}

		private static RelativityFileShare CreateRelativityFileShare(bool cloudInstance, int artifactId, string uncPath)
		{
			return new RelativityFileShare(
				artifactId,
				"file share",
				uncPath,
				new ResourceServerType(artifactId, "invalid"),
				null,
				cloudInstance);
		}
	}
}