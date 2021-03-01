// ----------------------------------------------------------------------------
// <copyright file="AuditHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Xml.Linq;

	using Polly;
	using Relativity.Audit.Services.Interface.Query;
	using Relativity.Audit.Services.Interface.Query.Models.AuditObjectManagerUI;
	using Relativity.Audit.Services.Interface.Query.Models.AuditQuery;
	using Relativity.Services.Objects.DataContracts;

	public static class AuditHelper
	{
		private const string TimestampFieldName = "Timestamp";
		private const string DetailsFieldName = "Details";

		public enum AuditAction
		{
			/// <summary>
			/// Default, none audit action.
			/// </summary>
			None = 0,

			/// <summary>
			/// Create audit action.
			/// </summary>
			Create = 2,

			/// <summary>
			/// Move audit action.
			/// </summary>
			Move = 11,

			/// <summary>
			/// Import audit action.
			/// </summary>
			Import = 32,

			/// <summary>
			/// Export audit action.
			/// </summary>
			Export = 33,
		}

		public static async Task<IList<string>> GetAuditActionsForSpecificObjectAsync(
			IntegrationTestParameters parameters,
			DateTime actionExecutionTime,
			int numberOfLastAuditsToTake,
			int objectArtifactI)
		{
			var auditList = await GetAuditsForSpecificObjectAsync(
									parameters,
									actionExecutionTime,
									numberOfLastAuditsToTake,
									objectArtifactI)
								.ConfigureAwait(false);

			return auditList.Select(audit => audit.ActionName).ToList();
		}

		public static async Task<IList<AuditLogItem>> GetAuditsForSpecificObjectAsync(
			IntegrationTestParameters parameters,
			DateTime actionExecutionTime,
			int numberOfLastAuditsToTake,
			int objectArtifactId)
		{
			var request = GetRequestByArtifactId(objectArtifactId, actionExecutionTime);

			var results = await AuditHelper.GetLastAuditObjectForActionAsync(parameters, numberOfLastAuditsToTake, request)
							  .ConfigureAwait(false);

			IList<AuditLogItem> auditList = new List<AuditLogItem>();

			foreach (var audit in results)
			{
				var auditId = audit.Name;
				var timeStamp = (DateTime)audit.FieldValues[0].Value;

				auditList.Add(await GetAuditAsync(parameters, auditId, timeStamp).ConfigureAwait(false));
			}

			return auditList;
		}

		public static Task<IList<string>> GetAuditActionDetailsForObjectAsync(
			IntegrationTestParameters parameters,
			DateTime actionExecutionTime,
			AuditAction action,
			int minNumberOfAuditsToTake,
			int objectArtifactId)
		{
			async Task<IList<string>> GetDetails()
			{
				IList<AuditLogItem> allAuditsForDocument = await GetAuditsForSpecificObjectAsync(
															   parameters,
															   actionExecutionTime,
															   minNumberOfAuditsToTake,
															   objectArtifactId).ConfigureAwait(false);

				return allAuditsForDocument.Where(x => x.ActionID == (int)action).Select(x => x.Details.ToString()).ToList();
			}

			const int RetryAttempts = 3;
			const int WaitTimeInSeconds = 10;
			return Policy.HandleResult<IList<string>>(x => x.Count() < minNumberOfAuditsToTake).WaitAndRetryAsync(
							RetryAttempts,
							retry => TimeSpan.FromSeconds(retry * WaitTimeInSeconds)).ExecuteAsync(GetDetails);
		}

		public static Dictionary<string, string> GetAuditDetails(
			IntegrationTestParameters parameters,
			AuditHelper.AuditAction action,
			int userId)
		{
			SqlConnectionStringBuilder builder = IntegrationTestHelper.GetSqlConnectionStringBuilder(parameters);

			using (var connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText =
						$@"SELECT [Details] FROM [EDDS{parameters.WorkspaceId}].[EDDSDBO].[AuditRecord_PrimaryPartition] WHERE [Action] = '{(int)action}' AND [Details] <> '' AND [UserID] = '{userId}'";

					return XmlToDictionary((string)command.ExecuteScalar());
				}
			}
		}

		private static Dictionary<string, string> XmlToDictionary(string data)
		{
			var rootElement = XElement.Parse(data);
			var actionElement = (XElement)rootElement.FirstNode;

			var names = actionElement.Elements("item").Attributes("name").Select(n => n.Value);
			var values = actionElement.Elements("item").Select(v => v.Value);
			var list = names.Zip(values, (k, v) => new { k, v }).ToDictionary(item => item.k, item => item.v);
			return list;
		}

		private static async Task<IEnumerable<RelativityObject>> GetLastAuditObjectForActionAsync(
			IntegrationTestParameters parameters,
			int nrOfLastAuditsToTake,
			QueryRequest request)
		{
			var queryOptions = new AuditQueryOptions
			{
				ReturnRawDetails = true,
			};

			async Task<IEnumerable<RelativityObject>> QueryAuditAsync()
			{
				using (var auditObjectManager = ServiceHelper.GetServiceProxy<IAuditObjectManagerUIService>(parameters))
				{
					QueryResult result = await auditObjectManager.QueryAsync(
											 parameters.WorkspaceId,
											 request,
											 1,
											 ServiceHelper.MaxItemsToFetch,
											 queryOptions).ConfigureAwait(false);

					return result.TotalCount == 0 ? Enumerable.Empty<RelativityObject>() : result.Objects.Take(nrOfLastAuditsToTake);
				}
			}

			const int RetryAttempts = 3;
			const int WaitTimeInSeconds = 10;
			return await Policy.HandleResult<IEnumerable<RelativityObject>>(x => x?.Count() == 0).WaitAndRetryAsync(
						   RetryAttempts,
						   retry => TimeSpan.FromSeconds(retry * WaitTimeInSeconds)).ExecuteAsync(QueryAuditAsync)
					   .ConfigureAwait(false);
		}

		private static string ToAuditTimeFormat(this DateTime dt)
		{
			return dt.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'");
		}

		private static string ToAuditTimePattern(this DateTime dt)
		{
			return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff");
		}

		private static QueryRequest GetRequestByArtifactId(int artifactId, DateTime actionExecutionTime)
		{
			return new QueryRequest
			{
				Fields = new List<FieldRef>()
											   {
												   new FieldRef { Name = TimestampFieldName },
												   new FieldRef { Name = DetailsFieldName },
											   },
				Condition = $"(('{TimestampFieldName}' >= {actionExecutionTime.ToAuditTimeFormat()})) AND (('ArtifactID' == {artifactId}))",
				ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Code },
			};
		}

		private static async Task<AuditLogItem> GetAuditAsync(IntegrationTestParameters parameters, string id, DateTime timestamp)
		{
			using (var auditQueryService = ServiceHelper.GetServiceProxy<IAuditQueryService>(parameters))
			{
				var request = new GetAuditRequest
				{
					Id = id,
					Timestamp = timestamp.ToAuditTimePattern(),
				};

				try
				{
					return await auditQueryService.GetAuditAsync(parameters.WorkspaceId, request).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					IntegrationTestHelper.Logger.LogError(e, $"Couldn't retrieve audit information using request: {request}");
				}

				return null;
			}
		}
	}
}