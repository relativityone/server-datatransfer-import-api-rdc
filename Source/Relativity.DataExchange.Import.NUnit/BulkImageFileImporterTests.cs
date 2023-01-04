// -----------------------------------------------------------------------------------------------------
// <copyright file="BulkImageFileImporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System;
	using System.Net;

	using global::NUnit.Framework;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS;
	using kCura.WinEDDS.Api;
	using kCura.WinEDDS.Service;

	using Moq;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.WebApiVsKeplerSwitch;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="BulkImageFileImporter"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class BulkImageFileImporterTests : BulkFileImporterTestsBase
	{
		private Mock<IImageReader> mockImageReader;
		private ImageLoadFile args;
		private MockBulkImageFileImporter importer;

		[Test]
		public void ShouldBulkImport()
		{
			MassImportResults results = this.importer.TryBulkImport(OverwriteType.Both);
			Assert.That(results, Is.Not.Null);
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
			Assert.That(this.importer.PauseCalled, Is.EqualTo(0));
		}

		[Test]
		public void ShouldRetrySystemExceptions()
		{
			this.MockBulkImportManager
				.Setup(
					x => x.BulkImportImage(
						It.IsAny<int>(),
						It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo>(),
						It.IsAny<bool>())).Throws(new InvalidOperationException("bombed out"));
			InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
				() => this.importer.TryBulkImport(OverwriteType.Both));
			Assert.That(exception.Message, Is.EqualTo("bombed out"));
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
			Assert.That(this.importer.PauseCalled, Is.EqualTo(2));
		}

		[Test]
		public void ShouldEnsureTheBatchSizeIsLessThenTheMinimumBatchSize()
		{
			this.importer.MinimumBatch = 100;
			this.importer.BatchSize = 300;
			Assert.That(this.importer.BatchSize, Is.EqualTo(300));
			Assert.That(this.importer.MinimumBatch, Is.EqualTo(100));
			this.importer.MinimumBatch = 500;
			this.importer.BatchSize = 300;
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
			Assert.That(this.importer.MinimumBatch, Is.EqualTo(500));
		}

		protected override void OnSetup()
		{
			var webApiVsKeplerMock = new Mock<IWebApiVsKepler>();
			webApiVsKeplerMock.Setup(x => x.UseKepler()).Returns(true);
			ManagerFactory._webApiVsKeplerLazy = new Lazy<IWebApiVsKepler>(() => webApiVsKeplerMock.Object);
			ManagerFactory._currentUrl = AppSettings.Instance.WebApiServiceUrl;

			this.args = new ImageLoadFile();
			this.args.CaseInfo = new Relativity.DataExchange.Service.CaseInfo();
			this.args.CaseInfo.RootArtifactID = -1;
			this.args.Credential = new NetworkCredential();
			ManagerFactory._currentCredentials = this.args.Credential;
			ManagerFactory._connectionInfo = new KeplerServiceConnectionInfo(
				new Uri(AppSettings.Instance.WebApiServiceUrl),
				this.args.Credential);
			this.mockImageReader = new Mock<IImageReader>();
			this.importer = new MockBulkImageFileImporter(
				this.args,
				this.Context,
				this.IoReporter,
				this.MockLogger.Object,
				this.Guid,
				true,
				this.MockBulkImportManager.Object,
				this.mockImageReader.Object,
				this.TokenSource,
				Relativity.DataExchange.Service.ExecutionSource.Unknown);

			this.importer.SetImportBatchSize(500);
		}
	}
}