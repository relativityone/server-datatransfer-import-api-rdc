using System;
using System.Collections.Generic;
using System.Linq;
using kCura.NUnit.Integration;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Services
{
	internal class ObjectService
	{
		public static int CreateObject(int workspaceId, int artifactTypeId, IDictionary<string, object> fields)
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				CreateRequest request = new CreateRequest
				{
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
					FieldValues = fields.Keys.Select(key => new FieldRefValuePair
					{
						Field = new FieldRef { Name = key }, Value = fields[key]
					})
				};

				CreateResult result = objectManager.CreateAsync(workspaceId, request).GetAwaiter().GetResult();
				List<InvalidOperationException> innerExceptions = result.EventHandlerStatuses.Where(x => !x.Success)
					.Select(status => new InvalidOperationException(status.Message)).ToList();
				if (innerExceptions.Count == 0)
				{
					return result.Object.ArtifactID;
				}

				throw new AggregateException(
					$"Failed to create a new instance for {artifactTypeId} artifact type.", innerExceptions);
			}
		}

		public static void DeleteObjects(int workspaceId, IList<int> artifactIds)
		{
			foreach (int artifactId in artifactIds.ToList())
			{
				DeleteObject(workspaceId, artifactId);
				artifactIds.Remove(artifactId);
			}
		}

		public static void DeleteObject(int workspaceId, int artifactId)
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				DeleteRequest request = new DeleteRequest
				{
					Object = new RelativityObjectRef { ArtifactID = artifactId }
				};

				DeleteResult result = objectManager.DeleteAsync(workspaceId, request).GetAwaiter().GetResult();
				if (result.Report.DeletedItems.Count == 0)
				{
					throw new InvalidOperationException($"Failed to delete the {artifactId} object.");
				}
			}
		}

		public static int QueryArtifactTypeId(int workspaceId, string objectTypeName)
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef
					{
						Name = "Object Type"
					},

					Fields = new[]
					{
						new FieldRef
						{
							Name = "Artifact Type ID"
						}
					},

					Condition = $"'Name' == '{objectTypeName}'"
				};

				QueryResult result = objectManager.QueryAsync(workspaceId, queryRequest, 0, 1).GetAwaiter().GetResult();
				if (result.TotalCount != 1)
				{
					return 0;
				}

				return (int)result.Objects.Single().FieldValues.Single().Value;
			}
		}
	}
}