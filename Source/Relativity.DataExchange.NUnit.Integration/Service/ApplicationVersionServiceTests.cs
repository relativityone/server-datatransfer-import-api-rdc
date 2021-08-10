// ----------------------------------------------------------------------------
// <copyright file="ApplicationVersionServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using kCura.WinEDDS.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	[TestFixture(true)]
	[TestFixture(false)]
	public class ApplicationVersionServiceTests
	{
		private readonly bool useKepler;
		private IntegrationTestParameters testParameters;

		private NetworkCredential credential;
		private RelativityInstanceInfo relativityInstanceInfo;
		private ILog logger;
		private Func<string> correlationIdFunc;

		public ApplicationVersionServiceTests(bool useKepler)
		{
			this.useKepler = useKepler;
		}

		[OneTimeTearDown]
		public static void OneTimeTearDown()
		{
			AppSettings.Instance.UseKepler = null;
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
																			 | SecurityProtocolType.Tls11
																			 | SecurityProtocolType.Tls12;

			this.testParameters = AssemblySetup.TestParameters;
			Assume.That(this.testParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			AppSettings.Instance.UseKepler = this.useKepler;
		}

		[SetUp]
		public void Setup()
		{
			this.credential = new NetworkCredential(
				this.testParameters.RelativityUserName,
				this.testParameters.RelativityPassword);
			this.relativityInstanceInfo = new RelativityInstanceInfo
			{
				Credentials = this.credential,
				CookieContainer = new CookieContainer(),
				WebApiServiceUrl = new Uri(AppSettings.Instance.WebApiServiceUrl),
			};
			this.logger = new TestNullLogger();
			string correlationId = Guid.NewGuid().ToString();
			this.correlationIdFunc = () => correlationId;
		}

		[Test]
		public async Task ShouldRetrieveRelativityVersion()
		{
			// Arrange
			IApplicationVersionService sut = ManagerFactory.CreateApplicationVersionService(
				this.relativityInstanceInfo,
				AppSettings.Instance,
				this.logger,
				this.correlationIdFunc);

			// Act
			Version actualVersion = await sut.GetRelativityVersionAsync(new CancellationToken(false))
										.ConfigureAwait(false);

			// Assert
			Assert.That(actualVersion, Is.Not.Null);
		}

		[Test]
		public void ShouldFailToRetrieveRelativityVersionWhenInvalidCredentialsPassed()
		{
			// Arrange
			this.credential.UserName = "INVALID USERNAME";
			this.credential.Password = "INVALID PASSWORD";

			const string ExpectedErrorMessage =
				@"The 'query Relativity get version' HTTP Web service 'POST' method failed with an HTTP 401 status code.

Error: Failed to retrieve the Relativity version. Contact your system administrator for assistance if this problem persists.

Detail: This error is considered fatal and suggests the client is unauthorized from making the HTTP Web service call and is likely a problem with expired credentials or authentication.";

			IApplicationVersionService sut = ManagerFactory.CreateApplicationVersionService(
				this.relativityInstanceInfo,
				AppSettings.Instance,
				this.logger,
				this.correlationIdFunc);

			// Act
			var exception = Assert.ThrowsAsync<HttpServiceException>(
				async () => await sut.GetRelativityVersionAsync(new CancellationToken(false)).ConfigureAwait(false));

			// Assert
			Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
			Assert.That(exception.Fatal, Is.True);
			Assert.That(exception.Message, Is.EqualTo(ExpectedErrorMessage));
		}

		[Test]
		public void ShouldFailToRetrieveRelativityVersionWhenInvalidWebApiUri()
		{
			// Arrange
			this.relativityInstanceInfo.WebApiServiceUrl = new Uri("https://invaliduri");

			const string ExpectedErrorMessage =
				@"The 'query Relativity get version' HTTP Web service 'POST' method failed with a web exception 1 status code.

Error: Failed to retrieve the Relativity version. Contact your system administrator for assistance if this problem persists.

Detail: The remote name could not be resolved: 'invaliduri'";

			IApplicationVersionService sut = ManagerFactory.CreateApplicationVersionService(
				this.relativityInstanceInfo,
				AppSettings.Instance,
				this.logger,
				this.correlationIdFunc);

			// Act
			var exception = Assert.ThrowsAsync<HttpServiceException>(
				async () => await sut.GetRelativityVersionAsync(new CancellationToken(false)).ConfigureAwait(false));

			// Assert
			Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
			Assert.That(exception.Fatal, Is.False);
			Assert.That(exception.Message, Is.EqualTo(ExpectedErrorMessage));
		}
	}
}