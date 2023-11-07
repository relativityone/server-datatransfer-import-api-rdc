// <copyright file="KeplerManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service
{
	using System;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using kCura.WinEDDS.Mapping;
	using Moq;
	using Relativity.DataExchange.Service;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;
	using Relativity.Services.Exceptions;

	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	[TestFixture]
	public class KeplerManagerTests
	{
		private readonly Mock<IServiceProxyFactory> _serviceProxyFactoryMock = new Mock<IServiceProxyFactory>();
		private readonly Mock<IServiceExceptionMapper> _exceptionMapperMock = new Mock<IServiceExceptionMapper>();
		private readonly Mock<IBulkImportService> _bulkImportServiceMock = new Mock<IBulkImportService>();
		private int internalKeplerTimeout;
		private int batchInProgressWaitTimeInSeconds;
		private int batchInProgressNumberOfRetries;
		private KeplerManagerTestClass _sut;

		protected Func<string> CorrelationIdFunc { get; private set; }

		[SetUp]
		public void SetUp()
		{
			this.internalKeplerTimeout = AppSettings.Instance.InternalKeplerTimeoutInSeconds;
			this.batchInProgressWaitTimeInSeconds = AppSettings.Instance.BatchInProgressWaitTimeInSeconds;
			this.batchInProgressNumberOfRetries = AppSettings.Instance.BatchInProgressNumberOfRetries;
			AppSettings.Instance.InternalKeplerTimeoutInSeconds = 3;
			AppSettings.Instance.BatchInProgressWaitTimeInSeconds = 2;
			AppSettings.Instance.BatchInProgressNumberOfRetries = 2;
			this._serviceProxyFactoryMock.Setup(x => x.CreateProxyInstance<IBulkImportService>()).Returns(this._bulkImportServiceMock.Object);
			this._sut = new KeplerManagerTestClass(this._serviceProxyFactoryMock.Object, this._exceptionMapperMock.Object, this.CorrelationIdFunc);
		}

		[TearDown]
		public void TearDown()
		{
			AppSettings.Instance.InternalKeplerTimeoutInSeconds = this.internalKeplerTimeout;
			AppSettings.Instance.BatchInProgressWaitTimeInSeconds = this.batchInProgressWaitTimeInSeconds;
			AppSettings.Instance.BatchInProgressNumberOfRetries = this.batchInProgressNumberOfRetries;
			this._sut.Dispose();
		}

		[Test]
		public void ShouldRethrowWithImportExportExceptionWhenRetriesAreExhausted()
		{
			// arrange
			this._bulkImportServiceMock
				.SetupSequence(x => x.BulkImportNativeAsync(It.IsAny<int>(), It.IsAny<NativeLoadInfo>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Throws(new ConflictException("Batch In Progress"))
				.Throws(new ConflictException("Batch In Progress"))
				.Throws(new ConflictException("Batch In Progress"));

			// act
			var ex = Assert.Throws<ImportExportException>(
				() =>
					{
						this._sut.BulkImportNativeMock(
							It.IsAny<int>(),
							It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo>(),
							It.IsAny<bool>(),
							It.IsAny<bool>());
					});

			// assert
			Assert.That(ex.Message, Is.EqualTo($"Timeout occurred after {AppSettings.Instance.BatchInProgressNumberOfRetries} retries with {AppSettings.Instance.BatchInProgressWaitTimeInSeconds} seconds timeout. The server is still processing the request."));
		}

		[Test]
		public void ShouldNotThrowWithAnyExceptionWhenRetriesAreNotExhausted()
		{
			// arrange
			this._bulkImportServiceMock
				.SetupSequence(x => x.BulkImportNativeAsync(It.IsAny<int>(), It.IsAny<NativeLoadInfo>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Throws(new ConflictException("Batch In Progress"))
				.Throws(new ConflictException("Batch In Progress"))
				.ReturnsAsync(new MassImportResults());

			// act
			// assert
			Assert.DoesNotThrow(
				() =>
					{
						this._sut.BulkImportNativeMock(
							It.IsAny<int>(),
							It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo>(),
							It.IsAny<bool>(),
							It.IsAny<bool>());
					});
		}

		[Test]
		public void ShouldRetryWhenInternalKeplerTimeoutException()
		{
			// arrange
			this._bulkImportServiceMock
				.SetupSequence(x => x.BulkImportNativeAsync(It.IsAny<int>(), It.IsAny<NativeLoadInfo>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Throws(new TimeoutException())
				.Throws(new ConflictException("Batch In Progress"))
				.Throws(new ConflictException("Batch In Progress"));

			// act
			var ex = Assert.Throws<ImportExportException>(
				() =>
					{
						this._sut.BulkImportNativeMock(
							It.IsAny<int>(),
							It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo>(),
							It.IsAny<bool>(),
							It.IsAny<bool>());
					});

			// assert
			Assert.That(ex.Message, Is.EqualTo($"Timeout occurred after {AppSettings.Instance.BatchInProgressNumberOfRetries} retries with {AppSettings.Instance.BatchInProgressWaitTimeInSeconds} seconds timeout. The server is still processing the request."));
		}

		[Test]
		public void ShouldNotThrowWithDifferentExceptionWhenRetriesAreNotExhausted()
		{
			// arrange
			this._bulkImportServiceMock
				.SetupSequence(x => x.BulkImportNativeAsync(It.IsAny<int>(), It.IsAny<NativeLoadInfo>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Throws(new TimeoutException())
				.Throws(new ConflictException("Batch In Progress"))
				.ReturnsAsync(new MassImportResults());

			// act
			// assert
			Assert.DoesNotThrow(
				() =>
					{
						this._sut.BulkImportNativeMock(
							It.IsAny<int>(),
							It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo>(),
							It.IsAny<bool>(),
							It.IsAny<bool>());
					});
		}

		[Test]
		public void ShouldRethrowWithTaskCanceledExceptionWhenRetriesAreExhaustedBecauseOfTimeOutException()
		{
			// arrange
			this._bulkImportServiceMock
				.SetupSequence(x => x.BulkImportNativeAsync(It.IsAny<int>(), It.IsAny<NativeLoadInfo>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>()))
				.Throws(new TimeoutException())
				.Throws(new TimeoutException())
				.Throws(new TimeoutException());

			// act
			var ex = Assert.Throws<TaskCanceledException>(
				() =>
					{
						this._sut.BulkImportNativeMock(
							It.IsAny<int>(),
							It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo>(),
							It.IsAny<bool>(),
							It.IsAny<bool>());
					});

			// assert
			Assert.That(ex.Message, Is.EqualTo($"ExecuteAsync was canceled due to internal timeout {AppSettings.Instance.InternalKeplerTimeoutInSeconds}"));
		}
	}
}