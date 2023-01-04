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

	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class ApplicationVersionServiceTests : KeplerServiceTestBase
	{
		private int originalRetries;
		private int originalWait;

		public ApplicationVersionServiceTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public void ApplicationVersionServiceOneTimeSetup()
		{
			this.originalRetries = AppSettings.Instance.ReadRelativityVersionErrorNumberOfRetries;
			this.originalWait = AppSettings.Instance.ReadRelativityVersionErrorWaitTimeInSeconds;

			AppSettings.Instance.ReadRelativityVersionErrorNumberOfRetries = 1;
			AppSettings.Instance.ReadRelativityVersionErrorWaitTimeInSeconds = 1;
		}

		[OneTimeTearDown]
		public void ApplicationVersionServiceOneTimeTearDown()
		{
			AppSettings.Instance.ReadRelativityVersionErrorNumberOfRetries = this.originalRetries;
			AppSettings.Instance.ReadRelativityVersionErrorWaitTimeInSeconds = this.originalWait;
		}

		[IdentifiedTest("7da5b27f-9c88-4ee9-8164-ea4e7c77a443")]
		public async Task ShouldRetrieveRelativityVersion()
		{
			// Arrange
			IApplicationVersionService sut = ManagerFactory.CreateApplicationVersionService(
				this.RelativityInstanceInfo,
				AppSettings.Instance,
				this.Logger,
				this.CorrelationIdFunc);

			// Act
			Version actualVersion = await sut.GetRelativityVersionAsync(new CancellationToken(false))
										.ConfigureAwait(false);

			// Assert
			Assert.That(actualVersion, Is.Not.Null);
		}

		[IdentifiedTest("8a5d194a-657f-428b-8e79-816bb3bdd43a")]
		public void ShouldFailToRetrieveRelativityVersionWhenInvalidCredentialsPassed()
		{
			// Arrange
			this.Credential.UserName = "INVALID USERNAME";
			this.Credential.Password = "INVALID PASSWORD";

			string[] expectedErrorMessages =
			{
				"The 'query Relativity get version' HTTP Web service 'POST' method failed with an HTTP 401 status code.",
				"Error: Failed to retrieve the Relativity version. Contact your system administrator for assistance if this problem persists.",
				"Detail: This error is considered fatal and suggests the client is unauthorized from making the HTTP Web service call and is likely a problem with expired credentials or authentication.",
			};

			IApplicationVersionService sut = ManagerFactory.CreateApplicationVersionService(
				this.RelativityInstanceInfo,
				AppSettings.Instance,
				this.Logger,
				this.CorrelationIdFunc);

			// Act
			var exception = Assert.ThrowsAsync<HttpServiceException>(
				async () => await sut.GetRelativityVersionAsync(new CancellationToken(false)).ConfigureAwait(false));

			// Assert
			Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
			Assert.That(exception.Fatal, Is.True);
			Assert.That(exception.Message, Contains.Substring(expectedErrorMessages[0]));
			Assert.That(exception.Message, Contains.Substring(expectedErrorMessages[1]));
			Assert.That(exception.Message, Contains.Substring(expectedErrorMessages[2]));
		}

		[IdentifiedTest("f70b88dc-b6b1-4d5b-9d07-3f463ad32cd7")]
		public void ShouldFailToRetrieveRelativityVersionWhenInvalidWebApiUri()
		{
			// Arrange
			this.RelativityInstanceInfo.WebApiServiceUrl = new Uri("https://invaliduri");
			string[] expectedErrorMessages =
            {
                "The 'query Relativity get version' HTTP Web service 'POST' method failed with a web exception 1 status code.",
                "Error: Failed to retrieve the Relativity version. Contact your system administrator for assistance if this problem persists.",
                "Detail: The remote name could not be resolved: 'invaliduri'",
            };

			IApplicationVersionService sut = ManagerFactory.CreateApplicationVersionService(
				this.RelativityInstanceInfo,
				AppSettings.Instance,
				this.Logger,
				this.CorrelationIdFunc);

			// Act
			var exception = Assert.ThrowsAsync<HttpServiceException>(
				async () => await sut.GetRelativityVersionAsync(new CancellationToken(false)).ConfigureAwait(false));

			// Assert
			Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
			Assert.That(exception.Fatal, Is.False);
			Assert.That(exception.Message, Contains.Substring(expectedErrorMessages[0]));
			Assert.That(exception.Message, Contains.Substring(expectedErrorMessages[1]));
			Assert.That(exception.Message, Contains.Substring(expectedErrorMessages[2]));
		}
	}
}