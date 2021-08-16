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

	[TestFixture(true)]
	[TestFixture(false)]
	public class ApplicationVersionServiceTests : KeplerServiceTestBase
	{
		public ApplicationVersionServiceTests(bool useKepler)
			: base(useKepler)
		{
		}

		[Test]
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

		[Test]
		public void ShouldFailToRetrieveRelativityVersionWhenInvalidCredentialsPassed()
		{
			// Arrange
			this.Credential.UserName = "INVALID USERNAME";
			this.Credential.Password = "INVALID PASSWORD";

			const string ExpectedErrorMessage =
				@"The 'query Relativity get version' HTTP Web service 'POST' method failed with an HTTP 401 status code.

Error: Failed to retrieve the Relativity version. Contact your system administrator for assistance if this problem persists.

Detail: This error is considered fatal and suggests the client is unauthorized from making the HTTP Web service call and is likely a problem with expired credentials or authentication.";

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
			Assert.That(exception.Message, Is.EqualTo(ExpectedErrorMessage));
		}

		[Test]
		public void ShouldFailToRetrieveRelativityVersionWhenInvalidWebApiUri()
		{
			// Arrange
			this.RelativityInstanceInfo.WebApiServiceUrl = new Uri("https://invaliduri");

			const string ExpectedErrorMessage =
				@"The 'query Relativity get version' HTTP Web service 'POST' method failed with a web exception 1 status code.

Error: Failed to retrieve the Relativity version. Contact your system administrator for assistance if this problem persists.

Detail: The remote name could not be resolved: 'invaliduri'";

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
			Assert.That(exception.Message, Is.EqualTo(ExpectedErrorMessage));
		}
	}
}