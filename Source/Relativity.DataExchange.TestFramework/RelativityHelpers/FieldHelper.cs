// ----------------------------------------------------------------------------
// <copyright file="FieldHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Newtonsoft.Json.Linq;
	using Polly;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Interfaces.Field;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;
	using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

	/// <summary>
	/// Defines static helper methods to manage fields.
	/// </summary>
	public static class FieldHelper
	{
		public static Task<int> CreateSingleObjectFieldAsync(IntegrationTestParameters testParameters, string fieldName, int fieldObjectArtifactTypeId, int destinationRdoArtifactTypeId)
		{
			var request = new SingleObjectFieldRequest
			{
				Name = fieldName,
				AssociativeObjectType = new ObjectTypeIdentifier { ArtifactTypeID = fieldObjectArtifactTypeId },
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
			};

			return CreateFieldAsync(testParameters, request);
		}

		public static Task CreateMultiObjectFieldAsync(IntegrationTestParameters testParameters, string fieldName, int fieldObjectArtifactTypeId, int destinationRdoArtifactTypeId)
		{
			return RelativityVersionChecker.VersionIsLowerThan(testParameters, RelativityVersion.Indigo)
						? CreateMultipleObjectFieldUsingHttpClientAsync(testParameters, fieldName, fieldObjectArtifactTypeId, destinationRdoArtifactTypeId)
						: CreateMultipleObjectFieldUsingKeplerAsync(testParameters, fieldName, fieldObjectArtifactTypeId, destinationRdoArtifactTypeId);
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

			return CreateFieldAsync(testParameters, request);
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

			return CreateFieldAsync(testParameters, request);
		}

		public static async Task<int> CreateFixedLengthTextFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId, bool isOpenToAssociations, int length)
		{
			if (RelativityVersionChecker.VersionIsLowerThan(testParameters, RelativityVersion.Goatsbeard))
			{
				var request = FieldHelper.PrepareFixedLengthFieldRequest(
					testParameters.WorkspaceId,
					fieldName,
					destinationRdoArtifactTypeId,
					isOpenToAssociations,
					string.Empty,
					length,
					true);

				return await CreateFixedLengthTextFieldUsingHttpClientAsync(
					testParameters,
					request)
					.ConfigureAwait(false);
			}
			else
			{
				var request = new FixedLengthFieldRequest()
				{
					Name = fieldName,
					ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
					OpenToAssociations = isOpenToAssociations,
					HasUnicode = true,
					Length = length,
				};

				return await CreateFixedLengthTextFieldUsingKeplerAsync(
							   testParameters,
							   request)
						   .ConfigureAwait(false);
			}
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

		public static Task CreateFileFieldAsync(IntegrationTestParameters testParameters, string fieldName, int destinationRdoArtifactTypeId)
		{
			var request = new FileFieldRequest()
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
			};

			return CreateFieldAsync(testParameters, request);
		}

		public static async Task<int> CreateFieldAsync(IntegrationTestParameters parameters, BaseFieldRequest fieldRequest)
		{
			var supportedFieldTypesToCreateMethodMapping = new Dictionary<Type, Func<IFieldManager, Task<int>>>
			{
				[typeof(SingleObjectFieldRequest)] = (manager) => manager.CreateSingleObjectFieldAsync(parameters.WorkspaceId, fieldRequest as SingleObjectFieldRequest),
				[typeof(MultipleObjectFieldRequest)] = (manager) => manager.CreateMultipleObjectFieldAsync(parameters.WorkspaceId, fieldRequest as MultipleObjectFieldRequest),
				[typeof(SingleChoiceFieldRequest)] = (manager) => manager.CreateSingleChoiceFieldAsync(parameters.WorkspaceId, fieldRequest as SingleChoiceFieldRequest),
				[typeof(MultipleChoiceFieldRequest)] = (manager) => manager.CreateMultipleChoiceFieldAsync(parameters.WorkspaceId, fieldRequest as MultipleChoiceFieldRequest),
				[typeof(WholeNumberFieldRequest)] = (manager) => manager.CreateWholeNumberFieldAsync(parameters.WorkspaceId, fieldRequest as WholeNumberFieldRequest),
				[typeof(LongTextFieldRequest)] = (manager) => manager.CreateLongTextFieldAsync(parameters.WorkspaceId, fieldRequest as LongTextFieldRequest),
				[typeof(FixedLengthFieldRequest)] = (manager) => manager.CreateFixedLengthFieldAsync(parameters.WorkspaceId, fieldRequest as FixedLengthFieldRequest),
				[typeof(FileFieldRequest)] = (manager) => manager.CreateFileFieldAsync(parameters.WorkspaceId, fieldRequest as FileFieldRequest),
			};

			return await ExecuteMethodForFieldType(parameters, supportedFieldTypesToCreateMethodMapping, fieldRequest).ConfigureAwait(false);
		}

		public static async Task<int> UpdateFieldAsync(IntegrationTestParameters parameters, int fieldIdentifier, BaseFieldRequest fieldRequest)
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

			return await ExecuteMethodForFieldType(parameters, methodsWithReturnValues, fieldRequest).ConfigureAwait(false);
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

		public static int GetFieldArtifactId(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string fieldName)
		{
			QueryResult result = QueryFieldByNameAsync(parameters, logger, fieldName).ConfigureAwait(false).GetAwaiter().GetResult();

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

		public static string PrepareFixedLengthFieldRequest(
			int parentArtifactId,
			string name,
			int destinationRdoArtifactTypeId,
			bool openToAssociations,
			string width,
			int length,
			bool unicode)
		{
			var fixedLengthFieldRequest = ResourceFileHelper.GetResourceFolderPath("FixedLengthTextFieldRequest.json");
			JObject request = JObject.Parse(File.ReadAllText(fixedLengthFieldRequest));

			request["Parent Artifact"]["Artifact ID"] = parentArtifactId;
			request["Name"] = name;
			request["Object Type"]["Descriptor Artifact Type ID"] = destinationRdoArtifactTypeId;
			request["Open To Associations"] = openToAssociations;
			request["Width"] = width;
			request["Length"] = length;
			request["Unicode"] = unicode;

			if (destinationRdoArtifactTypeId == (int)ArtifactTypeID.Document)
			{
				request["Is Relational"] = false;
			}

			return request.ToString();
		}

		public static async Task<int> CreateFixedLengthTextFieldUsingHttpClientAsync(IntegrationTestParameters testParameters, string request)
		{
			string url =
				$"{testParameters.RelativityRestUrl.AbsoluteUri}/Relativity.REST/Workspace/{testParameters.WorkspaceId}/Field";

			var queryResult = HttpClientHelper.PostAsync(testParameters, new Uri(url), request).GetAwaiter().GetResult();

			JObject result = JObject.Parse(queryResult);
			int resultArtifactId = (int)result["Results"][0]["ArtifactID"];

			return await Task.FromResult(resultArtifactId).ConfigureAwait(false);
		}

		public static async Task<int> CreateFixedLengthTextFieldUsingKeplerAsync(IntegrationTestParameters testParameters, FixedLengthFieldRequest request)
		{
			return await CreateFieldAsync(testParameters, request).ConfigureAwait(false);
		}

		public static async Task EnsureWellKnownFieldsAsync(IntegrationTestParameters parameters)
		{
			WellKnownFields.ControlNumberId = QueryIdentifierFieldId(parameters, WellKnownArtifactTypes.DocumentArtifactTypeName);
			await UpdateFieldAsync(parameters, WellKnownFields.ControlNumberId, WellKnownFieldRequests.IdentifierFieldRequest).ConfigureAwait(false);
			await CreateOrUpdateFieldsAsync(parameters, WellKnownFieldRequests.NonSystemFieldRequests).ConfigureAwait(false);
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

		private static async Task<QueryResult> QueryFieldByNameAsync(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string fieldName)
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
				QueryResult queryResult = await objectManager.QueryAsync(parameters.WorkspaceId, queryRequest, 0, 1).ConfigureAwait(false);
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

		private static async Task CreateMultipleObjectFieldUsingHttpClientAsync(
			IntegrationTestParameters parameters,
			string fieldName,
			int fieldObjectArtifactTypeId,
			int destinationRdoArtifactTypeId)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			string createMultiObjectFieldJson = ResourceFileHelper.GetResourceFolderPath("CreateMultiObjectFieldInput.json");
			JObject request = JObject.Parse(File.ReadAllText(createMultiObjectFieldJson));
			request["Name"] = fieldName;
			request["Parent Artifact"]["Artifact ID"] = parameters.WorkspaceId;
			request["Associative Object Type"]["Descriptor Artifact Type ID"] = fieldObjectArtifactTypeId;
			request["Object Type"]["Descriptor Artifact Type ID"] = destinationRdoArtifactTypeId;
			if (destinationRdoArtifactTypeId == (int)Relativity.ArtifactType.Document)
			{
				request["Available In Field Tree"] = false;
			}

			string url = $"{parameters.RelativityRestUrl.AbsoluteUri}/Relativity.REST/Workspace/{parameters.WorkspaceId}/Field";
			await HttpClientHelper.PostAsync(parameters, new Uri(url), request.ToString()).ConfigureAwait(false);
		}

		private static async Task CreateMultipleObjectFieldUsingKeplerAsync(
			IntegrationTestParameters parameters,
			string fieldName,
			int fieldObjectArtifactTypeId,
			int destinationRdoArtifactTypeId)
		{
			var request = new MultipleObjectFieldRequest
			{
				Name = fieldName,
				AssociativeObjectType = new ObjectTypeIdentifier { ArtifactTypeID = fieldObjectArtifactTypeId },
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = destinationRdoArtifactTypeId },
			};

			await CreateFieldAsync(parameters, request).ConfigureAwait(false);
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
	}
}