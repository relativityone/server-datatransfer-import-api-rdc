// ----------------------------------------------------------------------------
// <copyright file="RdoHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Interfaces.ObjectType;
	using Relativity.Services.Interfaces.ObjectType.Models;
	using Relativity.Services.Interfaces.Shared;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Defines static helper methods to manage Relativity Dynamic Objects.
	/// </summary>
	public static class RdoHelper
	{
		public const int WorkspaceArtifactTypeId = 8;

		private static readonly RelativityVersion ObjectTypeManagerReleaseVersion = RelativityVersion.Blazingstar;

		public static Task<int> CreateObjectTypeAsync(IntegrationTestParameters parameters, string objectTypeName)
		{
			return CanUseObjectTypeManagerKepler(parameters)
					   ? CreateObjectTypeUsingKeplerAsync(parameters, objectTypeName)
					   : Task.FromResult(CreateObjectTypeUsingRSAPI(parameters, objectTypeName));
		}

		public static Task<int> CreateObjectTypeAsync(IntegrationTestParameters parameters, string objectTypeName, int workspaceId)
		{
			return CanUseObjectTypeManagerKepler(parameters)
				       ? CreateObjectTypeUsingKeplerAsync(parameters, objectTypeName, workspaceId)
				       : Task.FromResult(CreateObjectTypeUsingRSAPI(parameters, objectTypeName, workspaceId));
		}

		public static Task<ObjectType> CreateObjectTypeAsync(IntegrationTestParameters parameters, string objectTypeName, int workspaceId, int parentArtifactId)
		{
			return CreateObjectTypeUsingKeplerAsync(parameters, objectTypeName, workspaceId, parentArtifactId);
		}

		public static int CreateObjectTypeInstance(
			IntegrationTestParameters parameters,
			int artifactTypeId,
			IDictionary<string, object> fields)
		{
			return CreateObjectTypeInstance(parameters, artifactTypeId, fields, null);
		}

		public static int CreateObjectTypeInstance(
			IntegrationTestParameters parameters,
			int artifactTypeId,
			IDictionary<string, object> fields,
			int? parentArtifactId)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (IObjectManager objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				CreateRequest request = new CreateRequest
				{
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
					FieldValues = fields.Keys.Select(
													key => new FieldRefValuePair
													{
														Field = new FieldRef { Name = key },
														Value = fields[key],
													}),
				};
				if (parentArtifactId.HasValue)
				{
					request.ParentObject = new RelativityObjectRef { ArtifactID = parentArtifactId.Value };
				}

				Relativity.Services.Objects.DataContracts.CreateResult result =
					objectManager.CreateAsync(parameters.WorkspaceId, request).GetAwaiter().GetResult();
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

		public static void DeleteObject(IntegrationTestParameters parameters, int artifactId)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (IObjectManager objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				DeleteRequest request = new DeleteRequest
				{
					Object = new RelativityObjectRef { ArtifactID = artifactId },
				};

				Relativity.Services.Objects.DataContracts.DeleteResult result =
					objectManager.DeleteAsync(parameters.WorkspaceId, request).GetAwaiter().GetResult();
				if (result.Report.DeletedItems.Count == 0)
				{
					throw new InvalidOperationException($"Failed to delete the {artifactId} object.");
				}
			}
		}

		/// <summary>
		/// Deletes all RDOs of a given type from a test workspace.
		/// </summary>
		/// <param name="parameters">Test context parameters.</param>
		/// <param name="artifactTypeID">Type of objects to delete.</param>
		/// <returns><see cref="Task"/> which completes when all RDOs are deleted.</returns>
		public static async Task DeleteAllObjectsByTypeAsync(IntegrationTestParameters parameters, int artifactTypeID)
		{
			const int DeleteBatchSize = 250;

			// Deleting objects in a small batches is more stable than deleting all objects of a given type at one go.
			// Please see https://jira.kcura.com/browse/REL-496822 and https://jira.kcura.com/browse/REL-478746 for details.
			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				var queryAllObjectsRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef
					{
						ArtifactTypeID = artifactTypeID,
					},
				};

				while (true)
				{
					var existingArtifacts = await objectManager
						.QuerySlimAsync(parameters.WorkspaceId, queryAllObjectsRequest, start: 0, length: DeleteBatchSize)
						.ConfigureAwait(false);
					var objectRefs = existingArtifacts.Objects
						.Select(x => x.ArtifactID)
						.Select(x => new RelativityObjectRef { ArtifactID = x })
						.ToList();

					if (!objectRefs.Any())
					{
						return;
					}

					var massDeleteByIds = new MassDeleteByObjectIdentifiersRequest
					{
						Objects = objectRefs,
					};
					await objectManager.DeleteAsync(parameters.WorkspaceId, massDeleteByIds).ConfigureAwait(false);
				}
			}
		}

		public static int QueryArtifactTypeId(IntegrationTestParameters parameters, string objectTypeName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (IObjectManager objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef { Name = "Object Type" },
					Fields = new[] { new FieldRef { Name = "Artifact Type ID" } },
					Condition = $"'Name' == '{objectTypeName}'",
				};

				Relativity.Services.Objects.DataContracts.QueryResult result = objectManager
					.QueryAsync(parameters.WorkspaceId, queryRequest, 0, 1).GetAwaiter().GetResult();
				if (result.TotalCount != 1)
				{
					return 0;
				}

				return (int)result.Objects.Single().FieldValues.Single().Value;
			}
		}

		public static int QueryRelativityObjectCount(IntegrationTestParameters parameters, int artifactTypeId)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
				};
				Relativity.Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
					parameters.WorkspaceId,
					queryRequest,
					start: 1,
					length: 1).GetAwaiter().GetResult();
				return result.TotalCount;
			}
		}

		public static IList<RelativityObject> QueryRelativityObjects(
			IntegrationTestParameters parameters,
			int artifactTypeId,
			IEnumerable<string> fields)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					Fields = fields?.Select(x => new FieldRef { Name = x }),
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
				};
				Relativity.Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
					parameters.WorkspaceId,
					queryRequest,
					1,
					ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
				return result.Objects;
			}
		}

		public static IList<RelativityObject> QueryRelativityObjects(
			IntegrationTestParameters parameters,
			int artifactTypeId,
			string condition,
			IEnumerable<string> fields)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					Fields = fields?.Select(x => new FieldRef { Name = x }),
					ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
					Condition = condition,
				};

				Relativity.Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
					parameters.WorkspaceId,
					queryRequest,
					1,
					ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
				return result.Objects;
			}
		}

		public static RelativityObject ReadRelativityObject(
			IntegrationTestParameters parameters,
			int artifactId,
			IEnumerable<string> fields)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				ReadRequest readRequest = new ReadRequest
				{
					Fields = fields.Select(x => new FieldRef { Name = x }),
					Object = new RelativityObjectRef { ArtifactID = artifactId },
				};

				Relativity.Services.Objects.DataContracts.ReadResult result = client.ReadAsync(parameters.WorkspaceId, readRequest)
					.GetAwaiter().GetResult();
				return result.Object;
			}
		}

		public static async Task DeleteObjectTypeAsync(IntegrationTestParameters parameters, int artifactTypeId)
		{
			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectTypeManager>(parameters))
			{
				await objectManager.DeleteAsync(parameters.WorkspaceId, artifactTypeId).ConfigureAwait(false);
			}
		}

		private static async Task<ObjectType> CreateObjectTypeUsingKeplerAsync(IntegrationTestParameters parameters, string objectTypeName, int workspaceId, int parentArtifactId)
		{
			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectTypeManager>(parameters))
			{
				var request = new ObjectTypeRequest
				{
					Name = objectTypeName,
					ParentObjectType = new Securable<ObjectTypeIdentifier>(new ObjectTypeIdentifier { ArtifactTypeID = parentArtifactId }),
				};

				int newObjectId = await objectManager.CreateAsync(workspaceId, request).ConfigureAwait(false);

				ObjectTypeResponse objectTypeResponse = await objectManager.ReadAsync(workspaceId, newObjectId).ConfigureAwait(false);
				return new ObjectType(objectTypeResponse.ArtifactID, objectTypeResponse.ArtifactTypeID);
			}
		}

		private static async Task<int> CreateObjectTypeUsingKeplerAsync(IntegrationTestParameters parameters, string objectTypeName)
		{
			return (await CreateObjectTypeUsingKeplerAsync(parameters, objectTypeName, parameters.WorkspaceId, WorkspaceArtifactTypeId).ConfigureAwait(false)).ArtifactTypeId;
		}

		private static async Task<int> CreateObjectTypeUsingKeplerAsync(IntegrationTestParameters parameters, string objectTypeName, int workspaceId)
		{
			return (await CreateObjectTypeUsingKeplerAsync(parameters, objectTypeName, workspaceId, WorkspaceArtifactTypeId).ConfigureAwait(false)).ArtifactTypeId;
		}

		private static int CreateObjectTypeUsingRSAPI(IntegrationTestParameters parameters, string objectTypeName, int workspaceId)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

#pragma warning disable CS0618 // Type or member is obsolete
			using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				client.APIOptions.WorkspaceID = workspaceId;

				var query = new Query<kCura.Relativity.Client.DTOs.ObjectType>
				{
					Condition = new TextCondition("Name", TextConditionEnum.EqualTo, objectTypeName),
					Fields = FieldValue.AllFields,
				};
				Result<kCura.Relativity.Client.DTOs.ObjectType> objectType = client.Repositories.ObjectType.Query(query).Results.FirstOrDefault();

				if (objectType != null)
				{
					return objectType.Artifact.DescriptorArtifactTypeID.Value;
				}

				var objectTypeDto = new kCura.Relativity.Client.DTOs.ObjectType
				{
					Name = objectTypeName,
					ParentArtifactTypeID = 8,
					SnapshotAuditingEnabledOnDelete = true,
					Pivot = true,
					CopyInstancesOnWorkspaceCreation = false,
					Sampling = true,
					PersistentLists = false,
					CopyInstancesOnParentCopy = false,
				};
				int artifactId = client.Repositories.ObjectType.CreateSingle(objectTypeDto);

				kCura.Relativity.Client.DTOs.ObjectType createdObjectType = client.Repositories.ObjectType.ReadSingle(artifactId);
				return createdObjectType.DescriptorArtifactTypeID.Value;
			}
		}

		private static int CreateObjectTypeUsingRSAPI(IntegrationTestParameters parameters, string objectTypeName)
		{
			return CreateObjectTypeUsingRSAPI(parameters, objectTypeName, parameters.WorkspaceId);
		}

		private static bool CanUseObjectTypeManagerKepler(IntegrationTestParameters parameters) =>
			!RelativityVersionChecker.VersionIsLowerThan(parameters, ObjectTypeManagerReleaseVersion);
	}
}