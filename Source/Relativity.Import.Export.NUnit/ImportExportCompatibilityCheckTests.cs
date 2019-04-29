// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportExportCompatibilityCheckTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.Logging;

	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class ImportExportCompatibilityCheckTests
	{
		private Mock<IApplicationVersionService> relativityVersionServiceMock;
		private Mock<ILog> logMock;
		private RelativityInstanceInfo instanceInfo;
		private IObjectCacheRepository objectCacheRepository;

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
		}

		[TearDown]
		public void Teardown()
		{
			this.objectCacheRepository.Dispose();
		}

		[Test]
		[TestCase("0.0.0.0", "1.0")]
		[TestCase("10.0.0.0", "0.0")]
		public void ShouldThrowWhenTheRelativityOrImportExportWebApiVersionIsInvalid(string mockRelativityVersion, string mockWebApiVersion)
		{
			// arrange
			Version relativityVersion = Version.Parse(mockRelativityVersion);
			Version webApiVersion = Version.Parse(mockWebApiVersion);
			this.relativityVersionServiceMock.Setup(x => x.GetRelativityVersionAsync(CancellationToken.None)).Returns(Task.FromResult(relativityVersion));
			this.relativityVersionServiceMock.Setup(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None)).Returns(Task.FromResult(webApiVersion));
			ImportExportCompatibilityCheck subjectUnderTest = new ImportExportCompatibilityCheck(
				this.instanceInfo,
				this.relativityVersionServiceMock.Object,
				this.logMock.Object,
				new Version(9, 7, 0, 0),
				new Version(1, 0),
				this.objectCacheRepository);

			// act
			RelativityNotSupportedException exception = Assert.ThrowsAsync<RelativityNotSupportedException>(
				async () => await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false));

			// assert
			Assert.That(exception.RelativityVersion, Is.EqualTo(relativityVersion));
		}

		[Test]
		[TestCase("10.0.0.0", "9.7.228.0", false)]
		[TestCase("10.1.0.0", "10.0.1.0", false)]
		[TestCase("10.1.0.0", "10.0.0.1", false)]
		[TestCase("10.0.0.0", "10.0.0.0", true)]
		[TestCase("10.0.0.0", "10.1.0.0", true)]
		[TestCase("10.0.0.0", "10.0.1.0", true)]
		[TestCase("10.0.0.0", "10.0.0.1", true)]
		public async Task ShouldValidateWhenRelativityDoesNotSupportTheWebApiVersionCheck(string minRelativityVersion, string mockRelativityVersion, bool expectedResult)
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
				VersionConstants.RequiredWebApiVersion,
				this.objectCacheRepository);

			// act
			if (expectedResult)
			{
				await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
			}
			else
			{
				Assert.ThrowsAsync<RelativityNotSupportedException>(
					async () =>
						{
							await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
						});
			}

			// assert
			this.relativityVersionServiceMock.Verify(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None), Times.Once);
			this.relativityVersionServiceMock.Verify(x => x.GetRelativityVersionAsync(CancellationToken.None), Times.Once);
		}

		[Test]
		[TestCase("10.0.0.0", "9.7.228.0", "1.0", "1.0", true)]
		[TestCase("10.0.0.0", "9.7.228.0", "1.0", "1.5", true)]
		[TestCase("10.0.0.0", "9.7.228.0", "1.0", "2.0", false)]
		[TestCase("10.1.0.0", "10.1.0.0",  "1.0", "1.0", true)]
		[TestCase("10.1.0.0", "10.0.1.0",  "1.0", "1.0", true)]
		[TestCase("10.1.0.0", "10.0.0.1",  "1.0", "1.0", true)]
		[TestCase("10.1.0.0", "10.0.0.1",  "1.5", "1.0", true)]
		[TestCase("10.1.0.0", "10.0.0.1",  "1.0", "1.5", true)]
		[TestCase("10.1.0.0", "10.0.0.1",  "1.0", "2.0", false)]
		public async Task ShouldValidateWhenTheRelativityAndWebApiVersionsAreRetrievedAsync(
			string minRelativityVersion,
			string relativityVersion,
			string webApiVersion,
			string minWebApiVersion,
			bool expectedResult)
		{
			// arrange
			Version relativityVer = Version.Parse(relativityVersion);
			Version webApiVer = Version.Parse(webApiVersion);
			this.relativityVersionServiceMock.Setup(x => x.GetRelativityVersionAsync(CancellationToken.None)).Returns(Task.FromResult(relativityVer));
			if (webApiVer.Major == 0)
			{
				this.relativityVersionServiceMock.Setup(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None))
					.Throws(new HttpServiceException("Not found", HttpStatusCode.NotFound, false));
			}
			else
			{
				this.relativityVersionServiceMock.Setup(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None)).Returns(Task.FromResult(webApiVer));
			}

			ImportExportCompatibilityCheck subjectUnderTest = new ImportExportCompatibilityCheck(
				this.instanceInfo,
				this.relativityVersionServiceMock.Object,
				this.logMock.Object,
				new Version(minRelativityVersion),
				new Version(minWebApiVersion),
				this.objectCacheRepository);

			// act
			if (expectedResult)
			{
				await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
			}
			else
			{
				Assert.ThrowsAsync<RelativityNotSupportedException>(
					async () =>
						{
							await subjectUnderTest.ValidateAsync(CancellationToken.None).ConfigureAwait(false);
						});
			}

			// assert
			this.relativityVersionServiceMock.Verify(x => x.GetImportExportWebApiVersionAsync(CancellationToken.None), Times.Once);
			this.relativityVersionServiceMock.Verify(x => x.GetRelativityVersionAsync(CancellationToken.None), Times.Once);
		}
	}
}