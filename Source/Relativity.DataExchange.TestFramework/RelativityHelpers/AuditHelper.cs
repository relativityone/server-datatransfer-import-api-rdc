// ----------------------------------------------------------------------------
// <copyright file="AuditHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Newtonsoft.Json.Linq;

	using Polly;
	using Relativity.Audit.Services.Interface.Query;
	using Relativity.Audit.Services.Interface.Query.Models.AuditObjectManagerUI;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	public static class AuditHelper
	{
		private const string TimestampFieldName = "Timestamp";
		private const string DetailsFieldName = "Details";

		private static readonly Dictionary<AuditAction, int> AuditActionArtifactIdCache = new Dictionary<AuditAction, int>();

		public enum AuditAction
		{
			/// <summary>
			/// Export audit action.
			/// </summary>
			Export,

			/// <summary>
			/// Import audit action.
			/// </summary>
			Import,

			/// <summary>
			/// Move audit action.
			/// </summary>
			Move,
		}

		public static async Task<IEnumerable<Dictionary<string, string>>> GetLastAuditDetailsForActionAsync(
			IntegrationTestParameters parameters,
			AuditAction action,
			DateTime actionExecutionTime,
			int nrOfLastAuditsToTake,
			int userId)
		{
			var results = new List<Dictionary<string, string>>();
			IEnumerable<RelativityObject> auditObjects =
				await GetLastAuditObjectForActionAsync(parameters, action, actionExecutionTime, nrOfLastAuditsToTake, userId).ConfigureAwait(false);

			foreach (var auditObject in auditObjects)
			{
				string artifactId = auditObject.ArtifactID.ToString();
				string detailsJson = (string)auditObject.FieldValues.FirstOrDefault(x => x.Field.Name == DetailsFieldName)?.Value;
				switch (action)
				{
					case AuditAction.Export:
						var exportAudit = ToDictionaryWithItem(detailsJson, "export");
						exportAudit["ArtifactID"] = artifactId;
						results.Add(exportAudit);
						break;
					case AuditAction.Import:
						var importAudit = ToDictionaryWithItem(detailsJson, "import");
						importAudit["ArtifactID"] = artifactId;
						results.Add(importAudit);
						break;
					case AuditAction.Move:
						var moveAudit = ToDictionaryWithoutItem(detailsJson, "moveDetails");
						moveAudit["ArtifactID"] = artifactId;
						results.Add(moveAudit);
						break;
				}
			}

			return results;
		}

		private static async Task<IEnumerable<RelativityObject>> GetLastAuditObjectForActionAsync(
			IntegrationTestParameters parameters,
			AuditAction action,
			DateTime actionExecutionTime,
			int nrOfLastAuditsToTake,
			int userId)
		{
			int auditActionArtifactId = await GetAuditActionArtifactIdAsync(parameters, action).ConfigureAwait(false);

			var request = new QueryRequest
			{
				Fields = new List<FieldRef>
					{
						new FieldRef { Name = DetailsFieldName },
					},
				Condition = $"(('Action' == CHOICE {auditActionArtifactId})) AND (('{TimestampFieldName}' >= {actionExecutionTime.ToAuditTimeFormat()})) AND (('UserID' == {userId}))",
				RowCondition = string.Empty,
				Sorts = new List<Sort>
					{
						new Sort
						{
							Direction = SortEnum.Descending,
							FieldIdentifier = new FieldRef { Name = TimestampFieldName },
						},
					},
				ExecutingSavedSearchID = 0,
				ExecutingViewID = 0,
				ActiveArtifactID = 0,
				MaxCharactersForLongTextValues = 0,
			};

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
					if (result.TotalCount == 0)
					{
						return Enumerable.Empty<RelativityObject>();
					}

					return result.Objects.Take(nrOfLastAuditsToTake);
				}
			}

			const int RetryAttempts = 3;
			const int WaitTimeInSeconds = 10;
			return await Policy.HandleResult<IEnumerable<RelativityObject>>(x => x?.Count() == 0).WaitAndRetryAsync(
						   RetryAttempts,
						   retry => TimeSpan.FromSeconds(retry * WaitTimeInSeconds)).ExecuteAsync(QueryAuditAsync)
					   .ConfigureAwait(false);
		}

		private static Dictionary<string, string> ToDictionaryWithItem(string detailsJson, string action)
		{
			JObject details = JObject.Parse(detailsJson);
			JEnumerable<JToken> detailTokens;

			detailTokens = details["auditElement"][action]["item"].Children();

			return detailTokens.ToDictionary(
				token => (string)token["@name"],
				token => (string)token["#text"]);
		}

		private static Dictionary<string, string> ToDictionaryWithoutItem(string detailsJson, string action)
		{
			JObject details = JObject.Parse(detailsJson);
			JEnumerable<JToken> detailTokens;

			detailTokens = details["auditElement"][action].Children();

			return detailTokens.Select(x => x.Value<JProperty>()).ToDictionary(
				x => x.Name,
				x => (string)x.Value);
		}

		private static async Task<int> GetAuditActionArtifactIdAsync(
			IntegrationTestParameters parameters,
			AuditAction action)
		{
			if (!AuditActionArtifactIdCache.ContainsKey(action))
			{
				AuditActionArtifactIdCache[action] =
					await QueryAuditActionArtifactIdAsync(parameters, action).ConfigureAwait(false);
			}

			return AuditActionArtifactIdCache[action];
		}

		private static async Task<int> QueryAuditActionArtifactIdAsync(
			IntegrationTestParameters parameters,
			AuditAction action)
		{
			async Task<int> QueryArtifactId()
			{
				using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
				{
					QueryRequest queryRequest = new QueryRequest
					{
						Condition = $"'Name' == '{action.ToString()}'",
						Fields = new List<FieldRef>(),
						ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Code },
					};
					QueryResult result = await client.QueryAsync(
											 parameters.WorkspaceId,
											 queryRequest,
											 1,
											 ServiceHelper.MaxItemsToFetch).ConfigureAwait(false);
					if (result.TotalCount != 1)
					{
						throw new InvalidOperationException(
							$"Failed to retrieve artifact ID for {action.ToString()} audit action. Expected single artifact but {result.TotalCount} were found.");
					}

					return result.Objects[0].ArtifactID;
				}
			}

			const int RetryAttempts = 3;
			const int WaitTimeInSeconds = 3;
			return await Policy.Handle<InvalidOperationException>().WaitAndRetryAsync(
							RetryAttempts,
							retry => TimeSpan.FromSeconds(WaitTimeInSeconds)).ExecuteAsync(QueryArtifactId)
						.ConfigureAwait(false);
		}

		private static string ToAuditTimeFormat(this DateTime dt)
		{
			return dt.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'");
		}
	}
}