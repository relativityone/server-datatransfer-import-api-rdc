// <copyright file="TokenRefreshTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;
	using kCura.WinEDDS.Credentials;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.OAuth2Client.Interfaces;
	using Relativity.OAuth2Client.TokenProviders.ProviderFactories;
	using Relativity.Services.Security;
	using Relativity.Services.Security.Models;
	using Relativity.Testing.Identification;

	using Constants = Relativity.DataExchange.Constants;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Authentication]
	[TestType.EdgeCase]
	[TestExecutionCategory.CI]
	public class TokenRefreshTests
	{
		private readonly IntegrationTestParameters testParameters = IntegrationTestHelper.IntegrationTestParameters;
		private readonly ILog logger = Relativity.Logging.Log.Logger;

		private readonly Uri stsUri;

		private OAuth2Client oAuth2Client;

		public TokenRefreshTests()
		{
			var stsRelativeUri = new Uri("/Relativity/Identity/connect/token", UriKind.Relative);
			this.stsUri = new Uri(this.testParameters.RelativityUrl, stsRelativeUri);
		}

		[SetUp]
		public async Task SetUpAsync()
		{
			using (var oAuth2ClientManager = ServiceHelper.GetServiceProxy<IOAuth2ClientManager>(this.testParameters))
			{
				var createClientRequest = new OAuth2Client
				{
					Name = $"ImportAPI test {Guid.NewGuid():N}",
					Enabled = true,
					Flow = OAuth2Flow.ClientCredentials,
					ContextUser = 9, // admin
					AccessTokenLifetimeInMinutes = 1,
				};
				this.oAuth2Client = await oAuth2ClientManager.CreateAsync(createClientRequest).ConfigureAwait(false);
			}
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			if (this.oAuth2Client != null)
			{
				using (var oAuth2ClientManager =
					ServiceHelper.GetServiceProxy<IOAuth2ClientManager>(this.testParameters))
				{
					await oAuth2ClientManager.DeleteAsync(this.oAuth2Client.Id).ConfigureAwait(false);
				}
			}
		}

		[Test]
		public async Task TestTokenRefreshAsync()
		{
			// arrange
			const int ExpectedNumberOfDocuments = 250_000;

			TapiClientModeAvailabilityChecker.InitializeTapiClient(
				TapiClient.Direct, // TAPI does not refresh token in the web mode
				IntegrationTestHelper.IntegrationTestParameters);
			AppSettings.Instance.UseKepler = true;

			ImportDataSource<object[]> dataSource = new ImportDataSourceBuilder()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(WellKnownFields.ExtractedText, new TextValueSource(5000, useCache: false))
				.Build(ExpectedNumberOfDocuments);

			using (var tokenProvider = new TokenProvider(this.stsUri, this.oAuth2Client))
			{
				ImportAPI importApi = ExtendedImportAPI.CreateByTokenProvider(
					this.testParameters.RelativityWebApiUrl.ToString(),
					tokenProvider);
				ImportBulkArtifactJob importJob = importApi.NewNativeDocumentImportJob();
				importJob.Settings = NativeImportSettingsProvider.GetDefaultSettings();

				importJob.Settings.WebServiceURL = this.testParameters.RelativityWebApiUrl.ToString();
				importJob.Settings.CaseArtifactId = this.testParameters.WorkspaceId;

				importJob.OnFatalException += jobReport => this.logger.LogError(
					jobReport.FatalException,
					"Fatal ImportAPI exception.");
				importJob.OnComplete += jobReport => this.logger.LogInformation(
					"ImportAPI job imported {x} documents",
					jobReport.TotalRows);
				importJob.OnError += errorDetails => this.logger.LogError(
					"Error occurred during import: {errorDetails}",
					errorDetails);

				// We are waiting 55 second, so token is almost expired (it is valid for 60 seconds)
				await Task.Delay(TimeSpan.FromSeconds(55)).ConfigureAwait(false);

				// In Relativity token is valid for 5 additional minutes after expiration
				await Task.Delay(TimeSpan.FromMinutes(5)).ConfigureAwait(false);

				using (var dataReader = new ImportDataSourceToDataReaderAdapter<object[]>(dataSource))
				{
					importJob.SourceData = new SourceIDataReader { Reader = dataReader };

					// act
					importJob.Execute();
				}
			}

			// assert
			int actualNumberOfDocuments = RdoHelper.QueryRelativityObjectCount(this.testParameters, 10);
			Assert.That(actualNumberOfDocuments, Is.EqualTo(ExpectedNumberOfDocuments), "All documents should be imported");
		}

		private class TokenProvider : IRelativityTokenProvider, ICredentialsProvider, IDisposable
		{
			private readonly ClientTokenProviderFactory oauthClient;
			private readonly ITokenProvider tokenProvider;

			public TokenProvider(Uri stsUri, OAuth2Client client)
			{
				this.oauthClient = new ClientTokenProviderFactory(stsUri, client.Id, client.Secret);
				this.tokenProvider = this.oauthClient.GetTokenProvider("WebApi", new[] { "SystemUserInfo" });

				// IAPI uses that static property when refreshing token
				RelativityWebApiCredentialsProvider.Instance().SetProvider(this);
			}

			public string GetToken()
			{
				var token = this.tokenProvider.GetAccessTokenAsync().GetAwaiter().GetResult();
				Relativity.Logging.Log.Logger.LogInformation("Getting access token. {token}, {stacktrace}", token, Environment.StackTrace);
				return token;
			}

			public NetworkCredential GetCredentials()
			{
				var token = this.GetToken();
				return new NetworkCredential(Constants.OAuthWebApiBearerTokenUserName, token);
			}

			public Task<NetworkCredential> GetCredentialsAsync(CancellationToken cancellationToken)
			{
				return Task.FromResult(this.GetCredentials());
			}

			public void Dispose()
			{
				this.oauthClient.Dispose();
			}
		}
	}
}
