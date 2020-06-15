// ----------------------------------------------------------------------------
// <copyright file="SearchablePdfTestHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	public static class SearchablePdfTestHelper
	{
		public static void SetupTestData(IntegrationTestParameters testParameters)
		{
			var documentArtifactIds = GetUniqueDocumentArtifactIds(testParameters);
			var remoteFilePaths = UploadSamplePdfFiles(testParameters);
			InsertSamplePdfFilesToDatabase(testParameters, remoteFilePaths, documentArtifactIds);
		}

		private static List<long> GetUniqueDocumentArtifactIds(IntegrationTestParameters testParameters)
		{
			const string QueryString = "SELECT DISTINCT TOP (1000) DocumentArtifactID FROM [EDDSDBO].[File]";
			List<long> documentArtifactIds = new List<long>();

			using (SqlConnection connection = new SqlConnection(CreateConnectionString(testParameters)))
			using (SqlCommand command = new SqlCommand(QueryString, connection))
			{
				connection.Open();
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						documentArtifactIds.Add(Convert.ToInt64(reader["DocumentArtifactID"]));
					}
				}
			}

			return documentArtifactIds;
		}

		private static List<string> UploadSamplePdfFiles(IntegrationTestParameters testParameters)
		{
			var bridge = SetupTapiBridge(testParameters);

			List<string> sourcePaths = TestData.SampleSearchablePdfTestFiles.ToList();
			for (int order = 0; order < sourcePaths.Count; order++)
			{
				bridge.AddPath(sourcePaths[order], Guid.NewGuid().ToString(), order);
			}

			List<string> uploadedFiles = new List<string>();
			bridge.TapiProgress += (sender, args) =>
				{
					if (args.Successful)
					{
						uploadedFiles.Add(args.TargetFile);
					}
				};

			const bool KeepJobAlive = false;
			bridge.WaitForTransfers("Waiting...", "Success", "Error", KeepJobAlive);
			return uploadedFiles;
		}

		private static UploadTapiBridge2 SetupTapiBridge(IntegrationTestParameters testParameters)
		{
			string workspaceDirectory = $"EDDS{testParameters.WorkspaceId}";
			string targetPath = Path.Combine(testParameters.FileShareUncPath, workspaceDirectory);

			var parameters = new UploadTapiBridgeParameters2
			{
				Credentials =
					new NetworkCredential(
						testParameters.RelativityUserName,
						testParameters.RelativityPassword),
				FileShare = testParameters.FileShareUncPath,
				MaxJobParallelism = 1,
				MaxJobRetryAttempts = 1,
				PreserveFileTimestamps = false,
				WaitTimeBetweenRetryAttempts = 0,
				WebCookieContainer = new CookieContainer(),
				WebServiceUrl = testParameters.RelativityWebApiUrl.ToString(),
				WorkspaceId = testParameters.WorkspaceId,
				TargetPath = targetPath,
			};

			ITapiObjectService objectService = new TapiObjectService();
			objectService.SetTapiClient(parameters, TapiClient.Web);

			UploadTapiBridge2 bridge = new UploadTapiBridge2(
				parameters,
				new TestNullLogger(),
				new NullAuthTokenProvider(),
				CancellationToken.None);
			return bridge;
		}

		private static void InsertSamplePdfFilesToDatabase(IntegrationTestParameters testParameters, List<string> remoteFilePaths, List<long> documentArtifactIds)
		{
			const int SamplePdfSizeBytes = 12_527;
			const string QueryHeader = @"
			INSERT INTO [EDDSDBO].[File]
				([Guid]
				,[DocumentArtifactID]
				,[Filename]
				,[Order]
				,[Type]
				,[Rotation]
				,[Identifier]
				,[Location]
				,[InRepository]
				,[Size]
				,[Details]
				,[Billable])
			VALUES";

			var data = remoteFilePaths.Select(
				(path, i) => CreateSingleRow(
					Path.GetFileName(path),
					documentArtifactIds[i],
					$"SearchablePdfTest{i}.pdf",
					$"DOC{documentArtifactIds[i]}_PDF",
					path,
					SamplePdfSizeBytes));

			var command = QueryHeader + string.Join(",", data) + ";";
			RunCommand(command, CreateConnectionString(testParameters));
		}

		private static string CreateSingleRow(string guid, long artifactId, string fileName, string identifier, string location, int size)
		{
			return $@"
				('{guid}'
				,{artifactId}
				,'{fileName}'
				,0
				,{(int)FileType.Pdf}
				,-1
				,'{identifier}'
				,'{location}'
				,1
				,{size}
				,NULL
				,1)";
		}

		private static void RunCommand(string queryString, string connectionString)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			using (SqlCommand command = new SqlCommand(queryString, connection))
			{
				command.Connection.Open();
				command.ExecuteNonQuery();
			}
		}

		private static string CreateConnectionString(IntegrationTestParameters testParameters)
		{
			return $"Server={testParameters.SqlInstanceName};Database=EDDS{testParameters.WorkspaceId};User Id={testParameters.SqlAdminUserName};Password={testParameters.SqlAdminPassword};";
		}
	}
}