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

	using kCura.Relativity.Client.DTOs;

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
		}

		public static async Task<Dictionary<string, string>> GetLastAuditDetailsForActionAsync(
			IntegrationTestParameters parameters,
			AuditAction action,
			DateTime actionExecutionTime)
		{
			RelativityObject auditObject =
				await GetLastAuditObjectForActionAsync(parameters, action, actionExecutionTime).ConfigureAwait(false);
			string detailsJson = (string)auditObject.FieldValues.FirstOrDefault(x => x.Field.Name == DetailsFieldName)?.Value;

			return ToDictionary(detailsJson);
		}

		private static async Task<RelativityObject> GetLastAuditObjectForActionAsync(
			IntegrationTestParameters parameters,
			AuditAction action,
			DateTime actionExecutionTime)
		{
			int auditActionArtifactId = await GetAuditActionArtifactIdAsync(parameters, action).ConfigureAwait(false);

			var request = new QueryRequest
			{
				Fields = new List<FieldRef>
					{
						new FieldRef { Name = DetailsFieldName },
					},
				Condition = $"(('Action' == CHOICE {auditActionArtifactId})) AND (('{TimestampFieldName}' >= {actionExecutionTime.ToAuditTimeFormat()}))",
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

			async Task<RelativityObject> QueryAuditAsync()
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
						throw new InvalidOperationException(
							$"Failed to retrieve audit object for {action.ToString()} audit action");
					}

					return result.Objects[0];
				}
			}

			const int RetryAttempts = 3;
			const int WaitTimeInSeconds = 10;
			return await Policy.Handle<InvalidOperationException>().WaitAndRetryAsync(
						   RetryAttempts,
						   retry => TimeSpan.FromSeconds(WaitTimeInSeconds)).ExecuteAsync(QueryAuditAsync)
					   .ConfigureAwait(false);
		}

		private static Dictionary<string, string> ToDictionary(string detailsJson)
		{
			JObject details = JObject.Parse(detailsJson);
			JEnumerable<JToken> detailTokens = details["auditElement"]["export"]["item"].Children();
			return detailTokens.ToDictionary(
				token => (string)token["@name"],
				token => (string)token["#text"]);
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
			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					Condition = $"'{ArtifactTypeNames.ObjectType}' == 'Data Grid Audit' AND 'Name' == '{action.ToString()}'",
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
						$"Failed to retrieve artifact ID for {action.ToString()} audit action");
				}

				return result.Objects[0].ArtifactID;
			}
		}

		private static string ToAuditTimeFormat(this DateTime dt)
		{
			return dt.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'");
		}
	}
}