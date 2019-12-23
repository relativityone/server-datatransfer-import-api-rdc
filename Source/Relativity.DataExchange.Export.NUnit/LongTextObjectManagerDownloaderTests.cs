// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextObjectManagerDownloaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	using ViewFieldInfo = kCura.WinEDDS.ViewFieldInfo;

	[TestFixture]
	public sealed class LongTextObjectManagerDownloaderTests : IDisposable
	{
		private const int ValidArtifactAndFieldArtifactId = 10;
		private LongTextObjectManagerDownloader _instance;

		private Mock<IMetadataProcessingStatistics> metadataProcessingStatistics;
		private ExportFile exportSettings;
		private SafeIncrement safeIncrement;
		private Mock<ILongTextRepository> longTextRepository;
		private Mock<IDownloadProgressManager> downloadProgressManager;
		private Mock<IStatus> status;
		private Mock<ILog> logger;
		private Mock<IAppSettings> appSettings;
		private Mock<ILongTextStreamService> longTextStreamService;

		[SetUp]
		public void SetUp()
		{
			metadataProcessingStatistics = new Mock<IMetadataProcessingStatistics>();
			ViewFieldInfo[] fields = new QueryFieldFactory().GetAllDocumentFields().ToArray();

			exportSettings = new ExportFile(1)
			{
				SelectedTextFields = new ViewFieldInfo[0],
				SelectedViewFields = fields,
				CaseInfo = new CaseInfo { EnableDataGrid = true }
			};
			safeIncrement = new SafeIncrement();
			longTextRepository = new Mock<ILongTextRepository>();
			downloadProgressManager = new Mock<IDownloadProgressManager>();
			status = new Mock<IStatus>();
			logger = new Mock<ILog>();
			appSettings = new Mock<IAppSettings>();
			appSettings.SetupGet(a => a.ExportLongTextDataGridThreadCount).Returns(1);
			appSettings.SetupGet(a => a.ExportLongTextSqlThreadCount).Returns(1);
			longTextStreamService = new Mock<ILongTextStreamService>();

			RefreshInstance();
		}

		private void RefreshInstance()
		{
			this._instance = new LongTextObjectManagerDownloader(
				metadataProcessingStatistics.Object,
				exportSettings,
				safeIncrement,
				longTextRepository.Object,
				downloadProgressManager.Object,
				status.Object,
				logger.Object,
				appSettings.Object,
				longTextStreamService.Object);
		}

		[Test]
		public void ItShouldThrowArgumentExceptionWhenThreadsIsLessThan1()
		{
			// Arrange
			 appSettings = new Mock<IAppSettings>();
			 RefreshInstance();
			var longtextExportRequests = new List<LongTextExportRequest>()
				                             {
					                             LongTextExportRequest.CreateRequestForFullText(
						                             new ObjectExportInfo { ArtifactID = ValidArtifactAndFieldArtifactId },
						                             ValidArtifactAndFieldArtifactId,
						                             "somewhere")
				                             };

			// ACT
			Assert.ThrowsAsync<ArgumentException>(
				async () => await this._instance.DownloadAsync(longtextExportRequests, CancellationToken.None).ConfigureAwait(false));
		}

		[Test]
		public void ItShouldThrowArgumentNullExceptionWhenDownloadingAsync()
		{
			// ACT
			Assert.ThrowsAsync<ArgumentNullException>(
				async () => await this._instance.DownloadAsync(null, CancellationToken.None).ConfigureAwait(false));
		}

		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(-5)]
		public async Task ItShouldThrowArgumentExceptionWhenObjectArtifactIdIsInvalid(int artifactId)
		{
			// Arrange
			var longtextExportRequests = new List<LongTextExportRequest>()
											 {
												 LongTextExportRequest.CreateRequestForFullText(
													 new ObjectExportInfo { ArtifactID = artifactId },
													 ValidArtifactAndFieldArtifactId,
													 "somewhere")
											 };

			// ACT /
			await this._instance.DownloadAsync(longtextExportRequests, CancellationToken.None).ConfigureAwait(false);

			// Assert
			downloadProgressManager.Verify(m => m.MarkArtifactAsError(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		[TestCase(0)]
		[TestCase(-1)]
		[TestCase(-5)]
		public async Task ItShouldThrowArgumentExceptionWhenFieldArtifactIdIsInvalid(int fieldArtifactId)
		{
			// Arrange
			var longtextExportRequests = new List<LongTextExportRequest>()
				                             {
					                             LongTextExportRequest.CreateRequestForFullText(
						                             new ObjectExportInfo { ArtifactID = ValidArtifactAndFieldArtifactId },
						                             fieldArtifactId,
						                             "somewhere")
				                             };

			// ACT /
			await this._instance.DownloadAsync(longtextExportRequests, CancellationToken.None).ConfigureAwait(false);

			// ACT // Assert
			downloadProgressManager.Verify(m => m.MarkArtifactAsError(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
		}

		[Test]
		[TestCase("")]
		[TestCase(null)]
		[TestCase("    ")]
		public async Task ItShouldThrowArgumentExceptionWhenDestinationLocationIsInvalid(string destinationLocation)
		{
			// Arrange
			var longtextExportRequests = new List<LongTextExportRequest>()
				                             {
					                             LongTextExportRequest.CreateRequestForFullText(
						                             new ObjectExportInfo { ArtifactID = ValidArtifactAndFieldArtifactId },
						                             ValidArtifactAndFieldArtifactId,
						                             destinationLocation)
				                             };

			// ACT /
			await this._instance.DownloadAsync(longtextExportRequests, CancellationToken.None).ConfigureAwait(false);

			// ACT // Assert
			downloadProgressManager.Verify(m => m.MarkArtifactAsError(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
		}

		public void Dispose()
		{
			_instance.Dispose();
		}

		[TearDown]
		public void TearDown()
		{
			this.Dispose();
		}
	}
}