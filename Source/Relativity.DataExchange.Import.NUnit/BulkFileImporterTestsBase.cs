// -----------------------------------------------------------------------------------------------------
// <copyright file="BulkFileImporterTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Process;
	using Relativity.Logging;

	/// <summary>
	/// Represents a base class for bulk file importer tests.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public abstract class BulkFileImporterTestsBase
	{
		protected Mock<IBulkImportManager> MockBulkImportManager { get; set; }

		protected Mock<IProcessEventWriter> MockProcessEventWriter { get; set; }

		protected Mock<IProcessErrorWriter> MockProcessErrorWriter { get; set; }

		protected Mock<IAppSettings> MockAppSettings { get; set; }

		protected Mock<ILog> MockLogger { get; set; }

		protected Guid Guid { get; set; }

		protected ProcessContext Context { get; set; }

		protected IIoReporter IoReporter { get; set; }

		protected CancellationTokenSource TokenSource { get; set; }

		[SetUp]
		public void Setup()
		{
			this.Guid = new Guid("E09E18F3-D0C8-4CFC-96D1-FBB350FAB3E1");
			this.MockBulkImportManager = new Mock<IBulkImportManager>();
			this.MockBulkImportManager
				.Setup(
					x => x.BulkImportImage(
						It.IsAny<int>(),
						It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo>(),
						It.IsAny<bool>())).Returns(new kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults());
			this.MockBulkImportManager
				.Setup(
					x => x.BulkImportNative(
						It.IsAny<int>(),
						It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo>(),
						It.IsAny<bool>(),
						It.IsAny<bool>())).Returns(new kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults());
			this.MockBulkImportManager
				.Setup(
					x => x.BulkImportObjects(
						It.IsAny<int>(),
						It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo>(),
						It.IsAny<bool>())).Returns(new kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults());
			this.MockAppSettings = new Mock<IAppSettings>();
			this.MockAppSettings.SetupGet(x => x.IoErrorWaitTimeInSeconds).Returns(0);
			this.MockProcessErrorWriter = new Mock<IProcessErrorWriter>();
			this.MockProcessEventWriter = new Mock<IProcessEventWriter>();
			this.MockLogger = new Mock<ILog>();
			this.Context = new ProcessContext(
				this.MockProcessEventWriter.Object,
				this.MockProcessErrorWriter.Object,
				this.MockAppSettings.Object,
				this.MockLogger.Object);
			this.TokenSource = new CancellationTokenSource();
			this.IoReporter = new IoReporter(new IoReporterContext(), this.MockLogger.Object, this.TokenSource.Token);
			AppSettings.Instance.IoErrorWaitTimeInSeconds = 0;
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = "https://r1.kcura.com/RelativityWebAPI/";
			this.OnSetup();
		}

		protected abstract void OnSetup();
	}
}