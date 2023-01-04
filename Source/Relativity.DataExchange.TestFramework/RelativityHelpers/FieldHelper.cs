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

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Interfaces.Field;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	using ArtifactType = Relativity.ArtifactType;
	using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

	/// <summary>
	/// Defines static helper methods to manage fields.
	/// </summary>
	public static class FieldHelper
	{
		public static Task<int> CreateSingleObjectFieldAsync(IntegrationTestParameters testParameters, string fieldName, int objectArtifactTypeId, int associativeObjectArtifactTypeId)
		{
			if (testParameters == null)
			{
				throw new ArgumentException($"{nameof(testParameters)} parameter should not be null");
			}

			return CreateSingleObjectFieldAsync(testParameters, fieldName, objectArtifactTypeId, associativeObjectArtifactTypeId, testParameters.WorkspaceId);
		}

		// <summary>
		// This method adds single object field to the object type specified by<see cref= "objectArtifactTypeId" />.Object type referenced by that field is specified by <see cref = "associativeObjectArtifactTypeId" />.
		// </summary>
		public static Task<int> CreateSingleObjectFieldAsync(IntegrationTestParameters testParameters, string fieldName, int objectArtifactTypeId, int associativeObjectArtifactTypeId, int workspaceId)
		{
			if (!RelativityVersionChecker.VersionIsLowerThan(testParameters, RelativityVersion.Indigo))
			{
				var request = new SingleObjectFieldRequest
				{
					Name = fieldName,
					AssociativeObjectType = new ObjectTypeIdentifier { ArtifactTypeID = associativeObjectArtifactTypeId },
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = objectArtifactTypeId },
					AllowGroupBy = false,
					AllowPivot = false,
					AllowSortTally = false,
					IsRequired = false,
					IsLinked = false,
					OpenToAssociations = false,
					Width = 12,
					Wrapping = false,
				};

				return CreateFieldAsync(testParameters, request, workspaceId);
			}

			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				Name = fieldName,
				AssociativeObjectType = new kCura.Relativity.Client.DTOs.ObjectType
				{
					DescriptorArtifactTypeID = associativeObjectArtifactTypeId,
				},
				FieldTypeID = kCura.Relativity.Client.FieldType.SingleObject,
				AllowGroupBy = false,
				AllowPivot = false,
				AllowSortTally = true,
				IgnoreWarnings = true,
				IsRequired = false,
				Linked = false,
				OpenToAssociations = false,
				Width = "12",
				Wrapping = false,
			};

			if (objectArtifactTypeId == WellKnownArtifactTypes.DocumentArtifactTypeId)
			{
				field.AvailableInFieldTree = true;
			}

			return Task.FromResult(FieldHelper.CreateField(testParameters, objectArtifactTypeId, field, workspaceId));
		}

		public static Task<int> CreateMultiObjectFieldAsync(
			IntegrationTestParameters testParameters,
			string fieldName,
			int objectArtifactTypeId,
			int associativeObjectArtifactTypeId)
		{
			if (testParameters == null)
			{
				throw new ArgumentException($"{nameof(testParameters)} parameter should not be null");
			}

			return CreateMultiObjectFieldAsync(testParameters, fieldName, objectArtifactTypeId, associativeObjectArtifactTypeId, testParameters.WorkspaceId);
		}

		public static Task<int> CreateMultiObjectFieldAsync(IntegrationTestParameters testParameters, string fieldName, int objectArtifactTypeId, int associativeObjectArtifactTypeId, int workspaceId)
		{
			if (!RelativityVersionChecker.VersionIsLowerThan(testParameters, RelativityVersion.Indigo))
			{
				var request = new MultipleObjectFieldRequest
				{
					Name = fieldName,
					AssociativeObjectType = new ObjectTypeIdentifier { ArtifactTypeID = associativeObjectArtifactTypeId },
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = objectArtifactTypeId },
					AllowGroupBy = false,
					AllowPivot = false,
					IsRequired = false,
					Width = 12,
				};

				return FieldHelper.CreateFieldAsync(testParameters, request, workspaceId);
			}

			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				Name = fieldName,
				AssociativeObjectType = new kCura.Relativity.Client.DTOs.ObjectType
				{
					DescriptorArtifactTypeID = associativeObjectArtifactTypeId,
				},
				FieldTypeID = kCura.Relativity.Client.FieldType.MultipleObject,
				AllowGroupBy = false,
				AllowPivot = false,
				IgnoreWarnings = true,
				IsRequired = false,
				Width = "12",
			};

			if (objectArtifactTypeId == WellKnownArtifactTypes.DocumentArtifactTypeId)
			{
				field.AvailableInFieldTree = true;
			}

			return Task.FromResult(FieldHelper.CreateField(testParameters, objectArtifactTypeId, field, workspaceId));
		}

		public static Task<int> CreateSingleChoiceFieldAsync(
			IntegrationTestParameters testParameters,
			string fieldName,
			int destinationRdoArtifactTypeId,
			bool isOpenToAssociations)
		{
			if (testParameters == null)
			{
				throw new ArgumentException($"{nameof(testParameters)} parameter should not be null");
			}

			return CreateSingleChoiceFieldAsync(testParameters, fieldName, destinationRdoArtifactTypeId, isOpenToAssociations, testParameters.WorkspaceId);
		}

		public static Task<int> CreateSingleChoiceFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations, int workspaceId)
		{
			var request = new SingleChoiceFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
				HasUnicode = true,
			};

			return CreateFieldAsync(testParameters, request, workspaceId);
		}

		public static Task<int> CreateMultiChoiceFieldAsync(
			IntegrationTestParameters testParameters,
			string fieldName,
			int destinationRdoArtifactTypeId,
			bool isOpenToAssociations)
		{
			if (testParameters == null)
			{
				throw new ArgumentException($"{nameof(testParameters)} parameter should not be null");
			}

			return CreateMultiChoiceFieldAsync(testParameters, fieldName, destinationRdoArtifactTypeId, isOpenToAssociations, testParameters.WorkspaceId);
		}

		public static Task<int> CreateMultiChoiceFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations, int workspaceId)
		{
			var request = new MultipleChoiceFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
				HasUnicode = true,
			};

			return CreateFieldAsync(testParameters, request, workspaceId);
		}

		public static Task<int> CreateDateFieldAsync(IntegrationTestParameters testParameters, int objectArtifactTypeId, string fieldName)
		{
			if (!RelativityVersionChecker.VersionIsLowerThan(testParameters, RelativityVersion.Foxglove))
			{
				var request = new DateFieldRequest()
				{
					Name = fieldName,
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = objectArtifactTypeId },
					AllowSortTally = false,
					AllowGroupBy = false,
					AllowPivot = false,
					IsLinked = false,
					IsRequired = false,
					OpenToAssociations = false,
					Width = 12,
					Wrapping = true,
					Formatting = Formatting.Date,
				};

				return FieldHelper.CreateFieldAsync(testParameters, request);
			}

			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				Name = fieldName,
				FieldTypeID = kCura.Relativity.Client.FieldType.Date,
				AllowGroupBy = false,
				AllowPivot = false,
				AllowSortTally = false,
				IgnoreWarnings = true,
				IsRequired = false,
				Linked = false,
				OpenToAssociations = false,
				Width = "12",
				Wrapping = true,
			};

			return Task.FromResult(FieldHelper.CreateField(testParameters, objectArtifactTypeId, field));
		}

		public static Task<int> CreateDecimalFieldAsync(IntegrationTestParameters testParameters, int objectArtifactTypeId, string fieldName)
		{
			if (!RelativityVersionChecker.VersionIsLowerThan(testParameters, RelativityVersion.Foxglove))
			{
				var request = new DecimalFieldRequest()
				{
					Name = fieldName,
					ObjectType = new ObjectTypeIdentifier()
					{
						ArtifactTypeID = objectArtifactTypeId,
					},
					AllowPivot = false,
					AllowSortTally = false,
					AllowGroupBy = false,
					IsRequired = false,
					IsLinked = false,
					OpenToAssociations = false,
					Width = 12,
					Wrapping = true,
				};

				return FieldHelper.CreateFieldAsync(testParameters, request);
			}

			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				Name = fieldName,
				FieldTypeID = kCura.Relativity.Client.FieldType.Decimal,
				AllowGroupBy = false,
				AllowPivot = false,
				AllowSortTally = false,
				IgnoreWarnings = true,
				IsRequired = false,
				Linked = false,
				OpenToAssociations = false,
				Width = "12",
				Wrapping = true,
			};

			return Task.FromResult(FieldHelper.CreateField(testParameters, objectArtifactTypeId, field));
		}

		public static Task<int> CreateFixedLengthTextFieldAsync(IntegrationTestParameters testParameters, int objectArtifactTypeId, string fieldName, bool isOpenToAssociations, int length)
		{
			if (!RelativityVersionChecker.VersionIsLowerThan(testParameters, RelativityVersion.Goatsbeard))
			{
				var request = new FixedLengthFieldRequest()
				{
					Name = fieldName,
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = objectArtifactTypeId },
					OpenToAssociations = isOpenToAssociations,
					HasUnicode = false,
					Length = length,
					Wrapping = false,
					Width = 50,
					IsRequired = false,
					AllowHtml = false,
					AllowGroupBy = false,
					AllowSortTally = false,
					AllowPivot = false,
				};

				return FieldHelper.CreateFieldAsync(testParameters, request);
			}

			kCura.Relativity.Client.DTOs.Field field = new kCura.Relativity.Client.DTOs.Field
			{
				Name = fieldName,
				FieldTypeID = kCura.Relativity.Client.FieldType.FixedLengthText,
				AllowGroupBy = false,
				AllowHTML = false,
				AllowPivot = false,
				AllowSortTally = false,
				Length = length,
				IgnoreWarnings = true,
				IncludeInTextIndex = true,
				IsRequired = false,
				Linked = false,
				OpenToAssociations = isOpenToAssociations,
				Unicode = false,
				Width = string.Empty,
				Wrapping = false,
			};

			if (objectArtifactTypeId == WellKnownArtifactTypes.DocumentArtifactTypeId)
			{
				field.IsRelational = false;
			}

			return Task.FromResult(FieldHelper.CreateField(testParameters, objectArtifactTypeId, field));
		}

		public static Task<int> CreateLongTextFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations)
		{
			var request = new LongTextFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
				HasUnicode = true,
			};

			return CreateFieldAsync(testParameters, request);
		}

		public static Task CreateWholeNumberFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations)
		{
			var request = new WholeNumberFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
				OpenToAssociations = isOpenToAssociations,
			};

			return CreateFieldAsync(testParameters, request);
		}

		public static Task<int> CreateFileFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId)
		{
			var request = new FileFieldRequest()
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
			};

			return CreateFieldAsync(testParameters, request);
		}

		public static Task<int> CreateFieldAsync(IntegrationTestParameters parameters, BaseFieldRequest fieldRequest)
		{
			if (parameters == null)
			{
				throw new ArgumentException($"{nameof(parameters)} parameter should not be null");
			}

			return CreateFieldAsync(parameters, fieldRequest, parameters.WorkspaceId);
		}

		public static int CreateField(
			IntegrationTestParameters parameters,
			int workspaceObjectTypeId,
			kCura.Relativity.Client.DTOs.Field field)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			return CreateField(parameters, workspaceObjectTypeId, field, parameters.WorkspaceId);
		}

		public static int CreateField(
			IntegrationTestParameters parameters,
			int workspaceObjectTypeId,
			kCura.Relativity.Client.DTOs.Field field,
			int workspaceId)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (field == null)
			{
				throw new ArgumentNullException(nameof(field));
			}

#pragma warning disable CS0618 // Type or member is obsolete
			using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				client.APIOptions.WorkspaceID = workspaceId;
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
					return resultSet.Results[0].Artifact.ArtifactID;
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

		public static async Task<RelativityObject> QueryIdentifierRelativityObjectAsync(IntegrationTestParameters parameters, string artifactTypeName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var result = await QuerySingleFieldForType(
					             parameters,
					             $"'{ArtifactTypeNames.ObjectType}' == '{artifactTypeName}' AND '{FieldFieldNames.IsIdentifier}' == true")
				             .ConfigureAwait(false);

			if (result.TotalCount != 1)
			{
				throw new InvalidOperationException(
					$"Failed to retrieve the identifier Relativity object for the '{artifactTypeName}' artifact type.");
			}

			return result.Objects[0];
		}

		public static async Task<RelativityObject> QueryFieldForTypeByNameAsync(IntegrationTestParameters parameters, string artifactTypeName, string fieldName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var result = await QuerySingleFieldForType(
					          parameters,
					          $"'{ArtifactTypeNames.ObjectType}' == '{artifactTypeName}' AND '{FieldFieldNames.Name}' == '{fieldName}'")
				          .ConfigureAwait(false);
			if (result.TotalCount != 1)
			{
				throw new InvalidOperationException(
					$"Failed to retrieve the single Relativity object for the '{artifactTypeName}' artifact type and field name {fieldName}.");
			}

			return result.Objects[0];
		}

		public static Task<int> GetFieldArtifactIdAsync(IntegrationTestParameters parameters, string fieldName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			return GetFieldArtifactIdAsync(parameters, parameters.WorkspaceId, IntegrationTestHelper.Logger, fieldName);
		}

		public static async Task<int> GetFieldArtifactIdAsync(IntegrationTestParameters parameters, int workspaceId, Relativity.Logging.ILog logger, string fieldName)
		{
			QueryResult result = await QueryFieldByNameAsync(parameters, workspaceId, logger, fieldName).ConfigureAwait(false);

			if (result.TotalCount != 1)
			{
				throw new InvalidOperationException(
					$"Failed to retrieve the result for field {fieldName}.");
			}

			return result.Objects[0].ArtifactID;
		}

		public static async Task<Dictionary<string, List<string>>> GetFieldValuesAsync(IntegrationTestParameters testParameters, string fieldName, string identifierFieldName, int artifactTypeId)
		{
			var request = new Services.Objects.DataContracts.QueryRequest
			{
				ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
				Fields = new[]
											   {
												   new FieldRef { Name = identifierFieldName },
												   new FieldRef { Name = fieldName },
											   },
			};
			Services.Objects.DataContracts.QueryResult result;
			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(testParameters))
			{
				const int maxNumberOfResults = 4500;
				result = await objectManager.QueryAsync(testParameters.WorkspaceId, request, 0, maxNumberOfResults).ConfigureAwait(false);
				if (result.TotalCount > maxNumberOfResults)
				{
					throw new InvalidOperationException($"Not all results returned: Is: {maxNumberOfResults}, Should be: {result.TotalCount}");
				}
			}

			var fieldsValuesForDocuments = new Dictionary<string, List<string>>();
			foreach (RelativityObject document in result.Objects)
			{
				string documentId = document.FieldValues.First().Value.ToString();

				var listToInsert = new List<string>();
				fieldsValuesForDocuments[documentId] = listToInsert;

				object fieldValue = document.FieldValues.Skip(1).First().Value;
				if (fieldValue is Relativity.Services.Objects.DataContracts.Choice choice)
				{
					listToInsert.Add(choice.Name);
				}
				else if (fieldValue is IList<Relativity.Services.Objects.DataContracts.Choice> choices)
				{
					listToInsert.AddRange(choices.Select(x => x.Name));
				}
				else if (fieldValue is RelativityObjectValue objectValue)
				{
					listToInsert.Add(objectValue.Name);
				}
				else if (fieldValue is IList<RelativityObjectValue> objectValues)
				{
					listToInsert.AddRange(objectValues.Select(x => x.Name));
				}
			}

			return fieldsValuesForDocuments;
		}

		public static async Task EnsureWellKnownFieldsAsync(IntegrationTestParameters parameters)
		{
			WellKnownFields.ControlNumberId = QueryIdentifierFieldId(parameters, WellKnownArtifactTypes.DocumentArtifactTypeName);
			await UpdateFieldAsync(parameters, WellKnownFields.ControlNumberId, WellKnownFieldRequests.IdentifierFieldRequest).ConfigureAwait(false);
			await CreateOrUpdateFieldsAsync(parameters, WellKnownFieldRequests.NonSystemFieldRequests).ConfigureAwait(false);
		}

		private static Task<int> CreateFieldAsync(IntegrationTestParameters parameters, BaseFieldRequest fieldRequest, int workspaceId)
		{
			var supportedFieldTypesToCreateMethodMapping = new Dictionary<Type, Func<IFieldManager, Task<int>>>
			{
				[typeof(SingleObjectFieldRequest)] = (manager) => manager.CreateSingleObjectFieldAsync(workspaceId, fieldRequest as SingleObjectFieldRequest),
				[typeof(MultipleObjectFieldRequest)] = (manager) => manager.CreateMultipleObjectFieldAsync(workspaceId, fieldRequest as MultipleObjectFieldRequest),
				[typeof(SingleChoiceFieldRequest)] = (manager) => manager.CreateSingleChoiceFieldAsync(workspaceId, fieldRequest as SingleChoiceFieldRequest),
				[typeof(MultipleChoiceFieldRequest)] = (manager) => manager.CreateMultipleChoiceFieldAsync(workspaceId, fieldRequest as MultipleChoiceFieldRequest),
				[typeof(WholeNumberFieldRequest)] = (manager) => manager.CreateWholeNumberFieldAsync(workspaceId, fieldRequest as WholeNumberFieldRequest),
				[typeof(LongTextFieldRequest)] = (manager) => manager.CreateLongTextFieldAsync(workspaceId, fieldRequest as LongTextFieldRequest),
				[typeof(FixedLengthFieldRequest)] = (manager) => manager.CreateFixedLengthFieldAsync(workspaceId, fieldRequest as FixedLengthFieldRequest),
				[typeof(FileFieldRequest)] = (manager) => manager.CreateFileFieldAsync(workspaceId, fieldRequest as FileFieldRequest),
				[typeof(DateFieldRequest)] = (manager) => manager.CreateDateFieldAsync(workspaceId, fieldRequest as DateFieldRequest),
				[typeof(DecimalFieldRequest)] = (manager) => manager.CreateDecimalFieldAsync(workspaceId, fieldRequest as DecimalFieldRequest),
			};

			return ExecuteMethodForFieldType(parameters, supportedFieldTypesToCreateMethodMapping, fieldRequest);
		}

		private static Task<int> UpdateFieldAsync(IntegrationTestParameters parameters, int fieldIdentifier, BaseFieldRequest fieldRequest)
		{
			var supportedFieldTypesToCreateMethodMapping = new Dictionary<Type, Func<IFieldManager, Task>>
			{
				[typeof(SingleObjectFieldRequest)] = (manager) => manager.UpdateSingleObjectFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as SingleObjectFieldRequest),
				[typeof(MultipleObjectFieldRequest)] = (manager) => manager.UpdateMultipleObjectFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as MultipleObjectFieldRequest),
				[typeof(SingleChoiceFieldRequest)] = (manager) => manager.UpdateSingleChoiceFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as SingleChoiceFieldRequest),
				[typeof(MultipleChoiceFieldRequest)] = (manager) => manager.UpdateMultipleChoiceFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as MultipleChoiceFieldRequest),
				[typeof(WholeNumberFieldRequest)] = (manager) => manager.UpdateWholeNumberFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as WholeNumberFieldRequest),
				[typeof(LongTextFieldRequest)] = (manager) => manager.UpdateLongTextFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as LongTextFieldRequest),
				[typeof(FixedLengthFieldRequest)] = (manager) => manager.UpdateFixedLengthFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as FixedLengthFieldRequest),
				[typeof(FileFieldRequest)] = (manager) => manager.UpdateFileFieldAsync(parameters.WorkspaceId, fieldIdentifier, fieldRequest as FileFieldRequest),
			};

			Func<IFieldManager, Task<int>> AddReturnValue(Func<IFieldManager, Task> f) => manager => f.Invoke(manager).ContinueWith((task) => fieldIdentifier);
			Dictionary<Type, Func<IFieldManager, Task<int>>> methodsWithReturnValues =
				supportedFieldTypesToCreateMethodMapping.ToDictionary(x => x.Key, x => AddReturnValue(x.Value));

			return ExecuteMethodForFieldType(parameters, methodsWithReturnValues, fieldRequest);
		}

		private static async Task<QueryResult> QueryFieldByNameAsync(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string fieldName)
		{
			return await QueryFieldByNameAsync(parameters, parameters.WorkspaceId, logger, fieldName).ConfigureAwait(false);
		}

		private static async Task<QueryResult> QueryFieldByNameAsync(IntegrationTestParameters parameters, int workspaceId, Relativity.Logging.ILog logger, string fieldName)
		{
			logger.LogVerbose("Querying for field.");
			using (IObjectManager objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest()
				{
					ObjectType = new ObjectTypeRef()
					{
						ArtifactTypeID = (int)ArtifactType.Field,
					},
					Condition = $"'Name' == '{fieldName}'",
				};
				QueryResult queryResult = await objectManager.QueryAsync(workspaceId, queryRequest, 0, 1).ConfigureAwait(false);
				return queryResult;
			}
		}

		private static async Task CreateOrUpdateFieldsAsync(IntegrationTestParameters parameters, IEnumerable<BaseFieldRequest> fieldRequests)
		{
			var createOrUpdateTaskList = fieldRequests.Select(fieldRequest => CreateOrUpdateFieldAsync(parameters, fieldRequest)).ToList();
			await Task.WhenAll(createOrUpdateTaskList).ConfigureAwait(false);
		}

		private static async Task<int> CreateOrUpdateFieldAsync(
			IntegrationTestParameters parameters,
			BaseFieldRequest fieldRequest)
		{
			var results = await QueryFieldByNameAsync(
							  parameters,
							  new TestNullLogger(),
							  fieldRequest.Name).ConfigureAwait(false);

			if (results.TotalCount == 0)
			{
				return await CreateFieldAsync(parameters, fieldRequest)
					.ConfigureAwait(false);
			}

			return await UpdateFieldAsync(parameters, results.Objects[0].ArtifactID, fieldRequest)
					.ConfigureAwait(false);
		}

		private static Task<int> ExecuteMethodForFieldType(IntegrationTestParameters parameters, Dictionary<Type, Func<IFieldManager, Task<int>>> methodMapping, BaseFieldRequest fieldRequest)
		{
			Func<IFieldManager, Task<int>> method = GetMethodForFieldType(methodMapping, fieldRequest);
			return ExecuteMethod(parameters, method);
		}

		private static Func<IFieldManager, Task<int>> GetMethodForFieldType(Dictionary<Type, Func<IFieldManager, Task<int>>> methodMapping, BaseFieldRequest fieldRequest)
		{
			if (!methodMapping.TryGetValue(fieldRequest.GetType(), out var method))
			{
				throw new InvalidOperationException(
					"This method does not support requested field type. Please see source code of that method for more details.");
			}

			return method;
		}

		private static async Task<int> ExecuteMethod(IntegrationTestParameters parameters, Func<IFieldManager, Task<int>> method)
		{
			using (var fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(parameters))
			{
				int fieldId = await Policy.Handle<Exception>()
								  .WaitAndRetryAsync(3, retryNumber => TimeSpan.FromSeconds(3 ^ retryNumber))
								  .ExecuteAsync(() => method(fieldManager)).ConfigureAwait(false);
				return fieldId;
			}
		}

		private static async Task<QueryResult> QuerySingleFieldForType(IntegrationTestParameters parameters, string condition)
		{
			using (IObjectManager client = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				QueryRequest queryRequest = new QueryRequest
					                            {
						                            Condition = condition,
						                            Fields = new List<FieldRef>
							                                     {
								                                     new FieldRef { Name = FieldFieldNames.Name },
							                                     },
						                            ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Field },
					                            };
				return await client.QueryAsync(parameters.WorkspaceId, queryRequest, 1, ServiceHelper.MaxItemsToFetch)
					       .ConfigureAwait(false);
			}
		}

		private static RelativityObject QueryIdentifierRelativityObject(IntegrationTestParameters parameters, string artifactTypeName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			return QueryIdentifierRelativityObjectAsync(parameters, artifactTypeName).GetAwaiter().GetResult();
		}
	}
}