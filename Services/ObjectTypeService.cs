using System;
using System.Linq;
using System.Threading.Tasks;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Services
{
	internal class ObjectTypeService
	{
		private const string _OBJECT_TYPE_NAME = "Object Type";
		private const string _ARTIFACT_TYPE_ID_FIELD = "Artifact Type ID";
		private const string _NAME_FIELD = "Name";

		public static async Task<int> GetArtifactTypeId(int workspaceId, string objectTypeName)
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				var queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef
					{
						Name = _OBJECT_TYPE_NAME
					},
					Fields = new[]
					{
						new FieldRef
						{
							Name = _ARTIFACT_TYPE_ID_FIELD
						}
					},
					Condition = $"'{_NAME_FIELD}' == '{objectTypeName}'"
				};
				QueryResult result = await objectManager.QueryAsync(workspaceId, queryRequest, 0, 1);
				return (int)result.Objects.Single().FieldValues.Single().Value;
			}
		}

		public static int CreateObjectType(int workspaceId, string objectTypeName)
		{
			using (IRSAPIClient client = ServiceFactory.GetProxy<IRSAPIClient>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				Query<kCura.Relativity.Client.DTOs.ObjectType> query =
					new Query<kCura.Relativity.Client.DTOs.ObjectType>
				{
					Condition = new TextCondition("Name", TextConditionEnum.EqualTo, objectTypeName),
					Fields = FieldValue.AllFields
				};

				Result<kCura.Relativity.Client.DTOs.ObjectType> objectType = client.Repositories.ObjectType.Query(query).Results.FirstOrDefault();
				if (objectType != null)
				{
					return objectType.Artifact.ArtifactID;
				}

				const int parentArtifactTypeId = 8;
				kCura.Relativity.Client.DTOs.ObjectType objectTypeDto = new kCura.Relativity.Client.DTOs.ObjectType
				{
					Name = objectTypeName,
					ParentArtifactTypeID = parentArtifactTypeId,
					SnapshotAuditingEnabledOnDelete = true,
					Pivot = true,
					CopyInstancesOnWorkspaceCreation = false,
					Sampling = true,
					PersistentLists = false,
					CopyInstancesOnParentCopy = false
				};

				int objectTypeArtifactId = client.Repositories.ObjectType.CreateSingle(objectTypeDto);
				return objectTypeArtifactId;
			}
		}

		public static int QueryObjectTypeDescriptorId(int workspaceId, int artifactId)
		{
			kCura.Relativity.Client.DTOs.ObjectType objectType = new kCura.Relativity.Client.DTOs.ObjectType(artifactId)
			{
				Fields = FieldValue.AllFields
			};

			ResultSet<kCura.Relativity.Client.DTOs.ObjectType> resultSet;
			using (IRSAPIClient client = ServiceFactory.GetProxy<IRSAPIClient>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				resultSet = client.Repositories.ObjectType.Read(objectType);
			}

			int? descriptorArtifactTypeId = null;
			if (resultSet.Success && resultSet.Results.Any())
			{
				descriptorArtifactTypeId = resultSet.Results.First().Artifact.DescriptorArtifactTypeID;
			}

			if (!descriptorArtifactTypeId.HasValue)
			{
				throw new InvalidOperationException(
					"Failed to retrieve Object Type descriptor artifact type identifier.");
			}

			return descriptorArtifactTypeId.Value;
		}
	}
}