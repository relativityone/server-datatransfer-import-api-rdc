// ----------------------------------------------------------------------------
// <copyright file="FieldHelper.cs" company="Relativity ODA LLC">
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

	using Polly;

	using Relativity.Services.Interfaces.Field;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Defines static helper methods to manage fields.
	/// </summary>
	public static class FieldHelper
	{
		public static Task CreateSingleObjectFieldAsync(IntegrationTestParameters testParameters, string fieldName, int fieldObjectArtifactTypeId, int destinationRdoArtifactTypeId)
		{
			var request = new SingleObjectFieldRequest
			{
				Name = fieldName,
				AssociativeObjectType = new ObjectTypeIdentifier { ArtifactTypeID = fieldObjectArtifactTypeId },
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
			};

			return TestFramework.RelativityHelpers.FieldHelper.CreateFieldAsync(testParameters, request);
		}

		public static Task CreateMultiObjectFieldAsync(IntegrationTestParameters testParameters, string fieldName, int fieldObjectArtifactTypeId, int destinationRdoArtifactTypeId)
		{
			var request = new MultipleObjectFieldRequest
			{
				Name = fieldName,
				AssociativeObjectType = new ObjectTypeIdentifier { ArtifactTypeID = fieldObjectArtifactTypeId },
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
			};

			return TestFramework.RelativityHelpers.FieldHelper.CreateFieldAsync(testParameters, request);
		}

		public static Task<int> CreateSingleChoiceFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations)
		{
			var request = new SingleChoiceFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
				HasUnicode = true,
			};

			return TestFramework.RelativityHelpers.FieldHelper.CreateFieldAsync(testParameters, request);
		}

		public static Task<int> CreateMultiChoiceFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations)
		{
			var request = new MultipleChoiceFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
				HasUnicode = true,
			};

			return TestFramework.RelativityHelpers.FieldHelper.CreateFieldAsync(testParameters, request);
		}

		public static Task CreateLongTextFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations)
		{
			var request = new LongTextFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
				HasUnicode = true,
			};

			return TestFramework.RelativityHelpers.FieldHelper.CreateFieldAsync(testParameters, request);
		}

		public static Task CreateWholeNumberFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations)
		{
			var request = new WholeNumberFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
			};

			return TestFramework.RelativityHelpers.FieldHelper.CreateFieldAsync(testParameters, request);
		}

		public static async Task<int> CreateFieldAsync<T>(IntegrationTestParameters parameters, T fieldRequest)
			where T : BaseFieldRequest
		{
			var supportedFieldTypesToCreateMethodMapping = new Dictionary<Type, Func<IFieldManager, BaseFieldRequest, int, Task<int>>>
			{
				[typeof(SingleObjectFieldRequest)] = (manager, request, workspaceId) => manager.CreateSingleObjectFieldAsync(workspaceId, request as SingleObjectFieldRequest),
				[typeof(MultipleObjectFieldRequest)] = (manager, request, workspaceId) => manager.CreateMultipleObjectFieldAsync(workspaceId, request as MultipleObjectFieldRequest),
				[typeof(SingleChoiceFieldRequest)] = (manager, request, workspaceId) => manager.CreateSingleChoiceFieldAsync(workspaceId, request as SingleChoiceFieldRequest),
				[typeof(MultipleChoiceFieldRequest)] = (manager, request, workspaceId) => manager.CreateMultipleChoiceFieldAsync(workspaceId, request as MultipleChoiceFieldRequest),
				[typeof(WholeNumberFieldRequest)] = (manager, request, workspaceId) => manager.CreateWholeNumberFieldAsync(workspaceId, request as WholeNumberFieldRequest),
				[typeof(LongTextFieldRequest)] = (manager, request, workspaceId) => manager.CreateLongTextFieldAsync(workspaceId, request as LongTextFieldRequest),
			};

			if (!supportedFieldTypesToCreateMethodMapping.ContainsKey(typeof(T)))
			{
				throw new InvalidOperationException("This method does not support creation of requested field. Please see source code of that method for more details.");
			}

			var createMethod = supportedFieldTypesToCreateMethodMapping[typeof(T)];

			using (var fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(parameters))
			{
				int fieldId = await Policy
							.Handle<Exception>()
							.WaitAndRetryAsync(3, retryNumber => TimeSpan.FromSeconds(3 ^ retryNumber))
							.ExecuteAsync(() => createMethod(fieldManager, fieldRequest, parameters.WorkspaceId))
							.ConfigureAwait(false);
				return fieldId;
			}
		}

		public static void CreateField(
			IntegrationTestParameters parameters,
			int workspaceObjectTypeId,
			kCura.Relativity.Client.DTOs.Field field)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (field == null)
			{
				throw new ArgumentNullException(nameof(field));
			}

			using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
			{
				client.APIOptions.WorkspaceID = parameters.WorkspaceId;
				List<kCura.Relativity.Client.DTOs.Field>
					fieldsToCreate = new List<kCura.Relativity.Client.DTOs.Field>();
				field.ObjectType = new kCura.Relativity.Client.DTOs.ObjectType
				{
					DescriptorArtifactTypeID = workspaceObjectTypeId,
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

		public static async Task DeleteFieldAsync(
			IntegrationTestParameters parameters,
			int fieldId)
		{
			using (IFieldManager fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(parameters))
			{
				await fieldManager.DeleteAsync(parameters.WorkspaceId, fieldId).ConfigureAwait(false);
			}
		}

		public static int QueryIdentifierFieldId(IntegrationTestParameters parameters, string artifactTypeName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			RelativityObject ro = QueryIdentifierRelativityObject(parameters, artifactTypeName);
			return ro.ArtifactID;
		}

		public static string QueryIdentifierFieldName(IntegrationTestParameters parameters, string artifactTypeName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			RelativityObject ro = QueryIdentifierRelativityObject(parameters, artifactTypeName);
			return ro.FieldValues[0].Value as string;
		}

		private static RelativityObject QueryIdentifierRelativityObject(IntegrationTestParameters parameters, string artifactTypeName)
		{
			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
				{
					Condition = $"'{ArtifactTypeNames.ObjectType}' == '{artifactTypeName}' AND '{FieldFieldNames.IsIdentifier}' == true",
					Fields = new List<FieldRef>
					{
						new FieldRef { Name = FieldFieldNames.Name },
					},
					ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Field },
				};
				Relativity.Services.Objects.DataContracts.QueryResult result = client.QueryAsync(
					parameters.WorkspaceId,
					queryRequest,
					1,
					ServiceHelper.MaxItemsToFetch).GetAwaiter().GetResult();
				if (result.TotalCount != 1)
				{
					throw new InvalidOperationException(
						$"Failed to retrieve the identifier Relativity object for the '{artifactTypeName}' artifact type.");
				}

				return result.Objects[0];
			}
		}
	}
}