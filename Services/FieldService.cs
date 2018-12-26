using System;
using FluentAssertions;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System.Collections.Generic;
using System.Linq;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Services
{
	internal class FieldService
	{
		public static string GetDocumentIdentifierFieldName(int workspaceId)
		{
			return GetIdentifierFieldName(workspaceId, ArtifactTypeNames.Document);
		}

		public static string GetIdentifierFieldName(int workspaceId, string artifactTypeName)
		{
			using (var client = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				var queryRequest = new QueryRequest
				{
					Condition = $"'{ArtifactTypeNames.ObjectType}' == '{artifactTypeName}' AND '{FieldFieldNames.IsIdentifier}' == true",
					Fields = new List<FieldRef>
					{
						new FieldRef { Name = FieldFieldNames.Name },
					},
					ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Field }
				};
				const int maxItemsToFetch = 2;
				QueryResult result = client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				result.TotalCount.Should().Be(1);
				return (string)result.Objects[0].FieldValues[0].Value;
			}
		}

		public static int GetIdentifierFieldId(int workspaceId, string artifactTypeName)
		{
			using (var client = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				var queryRequest = new QueryRequest
				{
					Condition = $"'{ArtifactTypeNames.ObjectType}' == '{artifactTypeName}' AND '{FieldFieldNames.IsIdentifier}' == true",
					ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Field }
				};
				const int maxItemsToFetch = 2;
				QueryResult result = client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				result.TotalCount.Should().Be(1);
				return result.Objects[0].ArtifactID;
			}
		}

		public static void CreateSingleObjectField(int workspaceId, int objectTypeId, int descriptorArtifactTypeId, string fieldName)
		{
			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				AllowGroupBy = false,
				AllowPivot = false,
				AllowSortTally = false,
				AssociativeObjectType = new kCura.Relativity.Client.DTOs.ObjectType { DescriptorArtifactTypeID = descriptorArtifactTypeId },
				FieldTypeID = kCura.Relativity.Client.FieldType.SingleObject,
				IgnoreWarnings = true,
				IsRequired = false,
				Linked = false,
				Name = fieldName,
				OpenToAssociations = false,
				Width = "12",
				Wrapping = false
			};

			CreateField(workspaceId, objectTypeId, field);
		}

		public static void CreateMultipleObjectField(int workspaceId, int objectTypeId, int descriptorArtifactTypeId, string fieldName)
		{
			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				AllowGroupBy = false,
				AllowPivot = false,
				AssociativeObjectType = new kCura.Relativity.Client.DTOs.ObjectType { DescriptorArtifactTypeID = descriptorArtifactTypeId },
				FieldTypeID = kCura.Relativity.Client.FieldType.MultipleObject,
				IgnoreWarnings = true,
				IsRequired = false,
				Name = fieldName,
				Width = "12"
			};

			CreateField(workspaceId, objectTypeId, field);
		}

		public static void CreateField(int workspaceId, int objectTypeId, kCura.Relativity.Client.DTOs.Field field)
		{
			using (IRSAPIClient client = ServiceFactory.GetProxy<IRSAPIClient>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				List<kCura.Relativity.Client.DTOs.Field>
					fieldsToCreate = new List<kCura.Relativity.Client.DTOs.Field>();
				field.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType
				{
					DescriptorArtifactTypeID = objectTypeId
				};

				kCura.Relativity.Client.DTOs.WriteResultSet<kCura.Relativity.Client.DTOs.Field> resultSet =
					client.Repositories.Field.Create(fieldsToCreate);
				resultSet = client.Repositories.Field.Create(field);
				if (resultSet.Success)
				{
					return;
				}

				List<Exception> innerExceptions = new List<Exception>();
				foreach (Result<kCura.Relativity.Client.DTOs.Field> result in resultSet.Results.Where(x => !x.Success))
				{
					innerExceptions.Add(new InvalidOperationException(result.Message));
				}

				throw new AggregateException(
					$"Failed to create the {field.Name} field. Error: {resultSet.Message}", innerExceptions);
			}
		}
	}
}
