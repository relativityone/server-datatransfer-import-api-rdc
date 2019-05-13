// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportExportCompatibilityCheckTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.Logging;

	[TestFixture("Processing")]
	[TestFixture(null)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class ImportExportCompatibilityCheckTests
	{
		private readonly string applicationName;
		private Mock<IApplicationVersionService> relativityVersionServiceMock;
		private Mock<ILog> logMock;
		private RelativityInstanceInfo instanceInfo;
		private IObjectCacheRepository objectCacheRepository;
		private Mock<IAppSettings> appSettings;
		private Mock<DataExchange.IAppSettingsInternal> appSettingsInternal;

		public ImportExportCompatibilityCheckTests(string applicationName)
		{
			this.applicationName = applicationName;
		}

		[SetUp]
		public void Setup()
		{
			this.relativityVersionServiceMock = new Mock<IApplicationVersionService>();
			this.logMock = new Mock<ILog>();
			this.instanceInfo = new RelativityInstanceInfo
			{
				Host = new Uri("https://relativity.one"),
				WebApiServiceUrl = new Uri("https://relativity.one/RelativityWebAPI/")
			};

			this.objectCacheRepository = new MemoryCacheRepository(TimeSpan.FromSeconds(0));
			this.appSettings = new Mock<IAppSettings>();
			this.appSettings.SetupGet(settings => settings.ApplicationName).Returns(this.applicationName);
			this.appSettingsInternal = new Mock<DataExchange.IAppSettingsInternal>();
			this.appSettingsInternal.SetupGet(settings => settings.EnforceVersionCompatibilityCheck).Returns(true);
		}

		[TearDown]
		public void Teardown()
		{
			this.objectCacheRepository.Dispose();
		}

		[Test]
		[TestCase("10.2.0.0", "1.0", "10.3.0.0", "1.0", "10.3.0.0")]
		[TestCase("10.3.0.0", "1.0", "10.3.0.0", "2.0", "10.3.0.0")]
		public async Task ShouldNotThrowWhenTheEnforceVersionCompatibilityCheckIsDisabledAsync(
			string mockRelativityVersion,
			string mockWebApiVersion,
			string mockMinRelativityVersion,
			string mockRequiredWebApiVersion,
			string webApiStartFromRelativityVersion)
		{
			// arrange
			Version relativityVersion = Version.Parse(mockRelativityVersion);
			Version webApiVersion = Version.Parse(mockWebApiVersion);
			this.appSettingsInternal.SetupGet(settings => settings.EnforceVersionCompatibilityCheck).Returns(false);
			this.relativityVersionServiceMock.Setup(x => x.GetRelativityVersionAsync(CancellationToken.None))
				.Returns(Task.FromResult(relativityVersion));
			this.relativityVersionServiceMock.Setup(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None))
				.Returns(Task.FromResult(webApiVersion));
			ImportExportCompatibilityCheck subjectUnderTest = new ImportExportCompatibilityCheck(
				this.instanceInfo,
				this.relativityVersionServiceMock.Object,
				this.logMock.Object,
				Version.Parse(mockMinRelativityVersion),
				Version.Parse(mockRequiredWebApiVersion),
				Version.Parse(webApiStartFromRelativityVersion),
				this.objectCacheRepository,
				this.appSettings.Object,
				this.appSettingsInternal.Object);

			// act
			await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);

			// assert
			if (Version.Parse(mockRelativityVersion) >= Version.Parse(mockMinRelativityVersion))
			{
				this.relativityVersionServiceMock.Verify(
					x => x.GetImportExportWebApiVersionAsync(CancellationToken.None),
					Times.Once);
			}

			this.relativityVersionServiceMock.Verify(x => x.GetRelativityVersionAsync(CancellationToken.None), Times.Once);
			this.logMock.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()));
		}

		[Test]
		[TestCase("0.0.0.0", "1.0")]
		[TestCase("10.3.0.0", "0.0")]
		public void ShouldThrowWhenTheRelativityOrImportExportWebApiVersionIsInvalid(
			string mockRelativityVersion,
			string mockWebApiVersion)
		{
			// arrange
			Version relativityVersion = Version.Parse(mockRelativityVersion);
			Version webApiVersion = Version.Parse(mockWebApiVersion);
			this.relativityVersionServiceMock.Setup(x => x.GetRelativityVersionAsync(CancellationToken.None))
				.Returns(Task.FromResult(relativityVersion));
			this.relativityVersionServiceMock.Setup(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None))
				.Returns(Task.FromResult(webApiVersion));
			ImportExportCompatibilityCheck subjectUnderTest = new ImportExportCompatibilityCheck(
				this.instanceInfo,
				this.relativityVersionServiceMock.Object,
				this.logMock.Object,
				new Version(9, 7, 0, 0),
				new Version(1, 0),
				new Version(10, 3),
				this.objectCacheRepository,
				this.appSettings.Object,
				this.appSettingsInternal.Object);

			// act
			RelativityNotSupportedException exception = Assert.ThrowsAsync<RelativityNotSupportedException>(
				async () => await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false));
			Console.WriteLine(exception.Message);

			// assert
			Assert.That(exception.RelativityVersion, Is.EqualTo(relativityVersion));
		}

		[Test]
		[TestCase("10.0.0.0", "9.7.228.0", "10.0.0.1", false)]
		[TestCase("10.1.0.0", "10.0.1.0", "10.1.0.1", false)]
		[TestCase("10.1.0.0", "10.0.0.1", "10.1.0.1", false)]
		[TestCase("10.0.0.0", "10.0.0.0", "10.0.0.1", true)]
		[TestCase("10.0.0.0", "10.1.0.0", "10.1.0.1", true)]
		[TestCase("10.0.0.0", "10.0.1.0", "10.0.1.1", true)]
		[TestCase("10.0.0.0", "10.0.0.1", "10.0.0.2", true)]
		public async Task ShouldValidateWhenRelativityDoesNotSupportTheWebApiVersionCheck(
		    string minRelativityVersion,
			string mockRelativityVersion,
			string webApiStartFromRelativityVersion,
			bool expectedResult)
		{
			// arrange
			Version relativityVersion = Version.Parse(mockRelativityVersion);
			this.relativityVersionServiceMock.Setup(x => x.GetRelativityVersionAsync(CancellationToken.None)).Returns(Task.FromResult(relativityVersion));
			this.relativityVersionServiceMock.Setup(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None))
				.Throws(new HttpServiceException("Not found", HttpStatusCode.NotFound, false));
			ImportExportCompatibilityCheck subjectUnderTest = new ImportExportCompatibilityCheck(
				this.instanceInfo,
				this.relativityVersionServiceMock.Object,
				this.logMock.Object,
				new Version(minRelativityVersion),
				DataExchange.VersionConstants.RequiredWebApiVersion,
				new Version(webApiStartFromRelativityVersion),
				this.objectCacheRepository,
				this.appSettings.Object,
				this.appSettingsInternal.Object);

			// act
			if (expectedResult)
			{
				await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
			}
			else
			{
				RelativityNotSupportedException exception = Assert.ThrowsAsync<RelativityNotSupportedException>(
					async () =>
						{
							await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
						});
				Console.WriteLine(exception.Message);
			}

			// assert
			this.relativityVersionServiceMock.Verify(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None), Times.Never);
			this.relativityVersionServiceMock.Verify(x => x.GetRelativityVersionAsync(CancellationToken.None), Times.Once);
		}

		[Test]
		[TestCase("9.7.228.0", "10.0.0.0", "10.0.0.0", "1.0", "2.0", false)]
		[TestCase("9.7.228.0", "10.0.0.0", "10.0.0.0", "1.0", "1.0", true)]
		[TestCase("9.7.228.0", "10.0.0.0", "10.0.0.0", "1.0", "1.2", true)]
		[TestCase("9.7.228.0", "10.2.0.0", "10.1.0.0", "1.0", "1.1", true)]
		[TestCase("9.7.228.0", "10.2.0.0", "10.1.0.0", "1.2", "1.0", true)]
		[TestCase("10.0.0.0", "10.2.0.0", "10.1.0.0", "1.2", "1.0", true)]
		[TestCase("10.0.0.0", "10.2.0.0", "10.1.0.0", "1.2", "2.0", false)]
		[TestCase("10.0.0.0", "10.2.0.0", "10.1.0.0", "2.0", "1.0", false)]
		[TestCase("10.0.0.0", "10.2.0.0", "10.1.0.0", "2.0", "1.2", false)]
		public async Task ShouldValidateWhenTheRelativityAndWebApiVersionsAreRetrievedAsync(
			string minRelativityVersion,
			string relativityVersion,
			string webApiStartFromRelativityVersion,
			string webApiVersion,
			string minWebApiVersion,
			bool expectedResult)
		{
			// arrange
			Version relativityVer = Version.Parse(relativityVersion);
			Version webApiVer = Version.Parse(webApiVersion);
			this.relativityVersionServiceMock.Setup(x => x.GetRelativityVersionAsync(CancellationToken.None)).Returns(Task.FromResult(relativityVer));
			this.relativityVersionServiceMock.Setup(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None)).Returns(Task.FromResult(webApiVer));

			ImportExportCompatibilityCheck subjectUnderTest = new ImportExportCompatibilityCheck(
				this.instanceInfo,
				this.relativityVersionServiceMock.Object,
				this.logMock.Object,
				new Version(minRelativityVersion),
				new Version(minWebApiVersion),
				new Version(webApiStartFromRelativityVersion),
				this.objectCacheRepository,
				this.appSettings.Object,
				this.appSettingsInternal.Object);

			// act
			if (expectedResult)
			{
				await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
			}
			else
			{
				RelativityNotSupportedException exception = Assert.ThrowsAsync<RelativityNotSupportedException>(
					async () =>
						{
							await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
						});
				Console.WriteLine(exception.Message);
			}

			// assert
			this.relativityVersionServiceMock.Verify(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None), Times.Once);
			this.relativityVersionServiceMock.Verify(x => x.GetRelativityVersionAsync(CancellationToken.None), Times.Once);
		}
	}
}