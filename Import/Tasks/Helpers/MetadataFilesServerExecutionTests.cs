using System;
using System.Net;
using System.Threading;
using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks.Helpers;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using BulkImportManager = kCura.WinEDDS.Service.BulkImportManager;

namespace kCura.WinEDDS.Core.NUnit.Import.Tasks.Helpers
{
	[TestFixture]
	public class MetadataFilesServerExecutionTests
	{
		private MetadataFilesServerExecution _instance;

		private ImportContext _importContext;
		private Mock<ITransferConfig> _transferConfig;
		private Mock<INativeLoadInfoFactory> _nativeLoadInfoFactory;
		private Mock<IBulkImportManager> _bulkImportManager;
		private Mock<ICancellationProvider> _cancellationProvider;
		private Mock<ILog> _log;

		[SetUp]
		public void SetUp()
		{
			_importContext = new ImportContext();

			_transferConfig = new Mock<ITransferConfig>();
			_nativeLoadInfoFactory = new Mock<INativeLoadInfoFactory>();
			_bulkImportManager = new Mock<IBulkImportManager>();

			var bulkImportStatisticsHandler = new Mock<IBulkImportStatisticsHandler>();
			_cancellationProvider = new Mock<ICancellationProvider>();

			_log = new Mock<ILog>();
			_instance = new MetadataFilesServerExecution(_importContext, _transferConfig.Object, _nativeLoadInfoFactory.Object, _bulkImportManager.Object,
				bulkImportStatisticsHandler.Object, _cancellationProvider.Object);
		}

		[Test]
		public void ItShouldExecuteImportOnServer()
		{
			var metadataFilesInfo = new MetadataFilesInfo();
			NativeLoadInfo settings = new NativeLoadInfo();

			_transferConfig.Setup(x => x.IoErrorNumberOfRetries).Returns(1);
			_nativeLoadInfoFactory.Setup(x => x.Create(metadataFilesInfo, _importContext)).Returns(settings);

			// ACT
			_instance.Import(metadataFilesInfo);

			// ASSERT
			_nativeLoadInfoFactory.Verify(x => x.Create(metadataFilesInfo, _importContext), Times.Once);
			_bulkImportManager.Verify(x => x.BulkImport(settings, _importContext), Times.Once);
		}

		[Test]
		[TestCaseSource(nameof(TimeoutAndPermissionExceptions))]
		public void ItShouldNotRetryForTimeoutAndPermissionExceptions(Exception e)
		{
			var metadataFilesInfo = new MetadataFilesInfo();
			NativeLoadInfo settings = new NativeLoadInfo();

			_transferConfig.Setup(x => x.IoErrorNumberOfRetries).Returns(3);
			_nativeLoadInfoFactory.Setup(x => x.Create(metadataFilesInfo, _importContext)).Returns(settings);

			_bulkImportManager.Setup(x => x.BulkImport(settings, _importContext)).Throws(e);

			// ACT
			Assert.That(() => _instance.Import(metadataFilesInfo), Throws.TypeOf(e.GetType()));

			// ASSERT
			_bulkImportManager.Verify(x => x.BulkImport(settings, _importContext), Times.Once);
		}

		[Test]
		public void ItShouldRetryException()
		{
			var metadataFilesInfo = new MetadataFilesInfo();
			NativeLoadInfo settings = new NativeLoadInfo();

			var numberOfRetires = 3;

			_transferConfig.Setup(x => x.IoErrorNumberOfRetries).Returns(numberOfRetires);
			_nativeLoadInfoFactory.Setup(x => x.Create(metadataFilesInfo, _importContext)).Returns(settings);

			_bulkImportManager.Setup(x => x.BulkImport(settings, _importContext)).Throws(new Exception());

			// ACT
			Assert.That(() => _instance.Import(metadataFilesInfo), Throws.Exception);

			// ASSERT
			_bulkImportManager.Verify(x => x.BulkImport(settings, _importContext), Times.Exactly(numberOfRetires));
		}

		[Test]
		public void ItShouldRespectCancellationToken()
		{
			var metadataFilesInfo = new MetadataFilesInfo();
			NativeLoadInfo settings = new NativeLoadInfo();

			Exception e = new Exception();

			_cancellationProvider.Setup(x => x.GetToken()).Returns(new CancellationToken(true));

			_transferConfig.Setup(x => x.IoErrorNumberOfRetries).Returns(3);
			_nativeLoadInfoFactory.Setup(x => x.Create(metadataFilesInfo, _importContext)).Returns(settings);

			_bulkImportManager.Setup(x => x.BulkImport(settings, _importContext)).Throws(e);

			// ACT
			Assert.That(() => _instance.Import(metadataFilesInfo), Throws.Exception);

			// ASSERT
			_bulkImportManager.Verify(x => x.BulkImport(settings, _importContext), Times.Once);
		}

		private static Exception[] TimeoutAndPermissionExceptions()
		{
			return new Exception[]
			{
				new BulkImportManager.BulkImportSqlTimeoutException(new SoapExceptionDetail()),
				new BulkImportManager.BulkImportSqlException(new SoapExceptionDetail()),
				new BulkImportManager.InsufficientPermissionsForImportException(""),
				new WebException("timed out")
			};
		}
	}
}