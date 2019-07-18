// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiObjectServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="TapiObjectService"/> integration tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Testing.Identification;
	using Relativity.Transfer;

	/// <summary>
	/// Represents <see cref="TapiObjectService"/> integration tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	public class TapiObjectServiceTests
	{
		private TapiBridgeParameters2 parameters;
		private Mock<Relativity.Logging.ILog> logger;
		private ITapiObjectService service;

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
			                                                                 | SecurityProtocolType.Tls11
			                                                                 | SecurityProtocolType.Tls12;
			IntegrationTestParameters testParameters = AssemblySetup.TestParameters.DeepCopy();
			this.logger = new Mock<ILog>();
			this.parameters = new TapiBridgeParameters2
				                  {
					                  Credentials = new NetworkCredential(
						                  testParameters.RelativityUserName,
						                  testParameters.RelativityPassword),
					                  WebCookieContainer = new CookieContainer(),
					                  WebServiceUrl = testParameters.RelativityWebApiUrl.ToString(),
					                  WorkspaceId = testParameters.WorkspaceId,
				                  };
			this.service = new TapiObjectService();
		}

		[IdentifiedTest("EE06BDD4-4A81-4499-B753-5225F2236793")]
		[Category(TestCategories.Integration)]
		public void ShouldCreateTheRelativityConnectionInfo()
		{
			RelativityConnectionInfo connectionInfo = this.service.CreateRelativityConnectionInfo(this.parameters);
			Assert.That(connectionInfo, Is.Not.Null);
			Assert.That(connectionInfo.Credential, Is.Not.Null);
			Assert.That(connectionInfo.Host, Is.Not.Null);
			Assert.That(connectionInfo.WebApiServiceUrl, Is.EqualTo(new Uri(this.parameters.WebServiceUrl)));
			Assert.That(connectionInfo.WebApiServiceCredential, Is.Null);
			Assert.That(connectionInfo.WorkspaceId, Is.EqualTo(this.parameters.WorkspaceId));

			// Sanity check.
			RelativityConnectionInfo connectionInfoDeepCopy = connectionInfo.DeepCopy();
			Assert.That(connectionInfo, Is.Not.SameAs(connectionInfoDeepCopy));
		}

		[IdentifiedTest("9B6904C0-8E74-4313-A679-3057AF0B7463")]
		[Category(TestCategories.Integration)]
		public void ShouldCreateTheRelativityTransferHost()
		{
			RelativityConnectionInfo connectionInfo = this.service.CreateRelativityConnectionInfo(this.parameters);
			using (ITransferLog log = new NullTransferLog())
			using (IRelativityTransferHost transferHost = this.service.CreateRelativityTransferHost(connectionInfo, log))
			{
				Assert.That(transferHost, Is.Not.Null);
			}
		}

		[IdentifiedTest("F3EE7892-EBDA-4107-BAD5-2BC33475247A")]
		[Category(TestCategories.Integration)]
		public async Task ShouldGetTheWorkspaceClientAsync()
		{
			Guid clientId = await this.service.GetWorkspaceClientIdAsync(this.parameters).ConfigureAwait(false);
			Assert.That(clientId, Is.Not.EqualTo(Guid.Empty));
		}

		[IdentifiedTest("0ADE3B31-30E1-4ACD-ABB3-7BAA13D0D329")]
		[Category(TestCategories.Integration)]
		public async Task ShouldGetTheWorkspaceClientDisplayNameAsync()
		{
			string name = await this.service.GetWorkspaceClientDisplayNameAsync(this.parameters).ConfigureAwait(false);
			Assert.That(name, Is.Not.Null.Or.Empty);
		}

		[IdentifiedTest("F0C1E073-FD9C-4BDA-A71A-43F7392A6B71")]
		[Category(TestCategories.Integration)]
		public async Task ShouldGetTheWorkspaceDefaultFileShareAsync()
		{
			RelativityFileShare defaultFileShare = await this.service.GetWorkspaceDefaultFileShareAsync(
				                                       this.parameters,
				                                       this.logger.Object,
				                                       CancellationToken.None).ConfigureAwait(false);
			Assert.That(defaultFileShare, Is.Not.Null);
			Assert.That(defaultFileShare.ArtifactId, Is.GreaterThan(0));
			Assert.That(defaultFileShare.Name, Is.Not.Null.Or.Empty);
			Assert.That(defaultFileShare.ResourceServerType, Is.Not.Null);
			Assert.That(defaultFileShare.Url, Is.Not.Null.Or.Empty);
		}

		[IdentifiedTest("8CD2E655-629C-45C5-84AF-9D4EBAD76551")]
		[Category(TestCategories.Integration)]
		public async Task ShouldSearchForFileStorageAsync()
		{
			ITapiFileStorageSearchResults results = await this.service.SearchFileStorageAsync(
				                                        this.parameters,
				                                        this.logger.Object,
				                                        CancellationToken.None).ConfigureAwait(false);
			Assert.That(results, Is.Not.Null);
			Assert.That(results.FileShares, Is.Not.Null);
			Assert.That(results.InvalidFileShares, Is.Not.Null);
		}
	}
}