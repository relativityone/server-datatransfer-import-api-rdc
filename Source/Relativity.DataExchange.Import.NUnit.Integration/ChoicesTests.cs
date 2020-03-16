// <copyright file="ChoicesTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	using OverlayBehavior = kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior;

	[TestFixture]
	public class ChoicesTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const int NumberOfDocumentsToImport = 10;
		private readonly string multiValueDelimiter = SettingsConstants.DefaultMultiValueDelimiter.ToString();

		private int originalImportBatchSize;
		private List<int> createdFieldsIds = new List<int>();

		[OneTimeSetUp]
		public Task OneTimeSetUpAsync()
		{
			this.originalImportBatchSize = AppSettings.Instance.ImportBatchSize;
			AppSettings.Instance.ImportBatchSize = 4;

			return RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			AppSettings.Instance.ImportBatchSize = this.originalImportBatchSize;
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			var deleteDocumentsTask = RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document);

			foreach (var createdFieldId in this.createdFieldsIds)
			{
				await FieldHelper.DeleteFieldAsync(this.TestParameters, createdFieldId).ConfigureAwait(false);
			}

			await deleteDocumentsTask.ConfigureAwait(false);

			this.createdFieldsIds = new List<int>();
		}

		[IdentifiedTestCase("0e72e6f1-6c53-45ca-9d5d-d9ccb46f2195", OverlayBehavior.MergeAll)]
		[IdentifiedTestCase("5fa9f2a6-3a8a-47c3-aeac-b558f3ad0c02", OverlayBehavior.ReplaceAll)]
		[IdentifiedTestCase("dc3a8ee9-dbcc-4444-b480-4c2912f4ff1f", OverlayBehavior.UseRelativityDefaults)]
		public async Task ShouldReplaceExistingValuesForSingleChoiceFieldAsync(OverlayBehavior overlayBehavior)
		{
			// ARRANGE
			this.InitializeExecutionContext(overlayBehavior);

			string choiceName = "SINGLE_CHOICE_FIELD";
			await this.CreateSingleChoiceFieldAsync(choiceName).ConfigureAwait(false);

			List<string> documentIdsList = GenerateIdsForDocuments(NumberOfDocumentsToImport);

			// import initial data to workspace
			string[] initialUniqueChoices = { "a", "b", "c", "d" };
			IEnumerable<string[]> initialChoicesList = GenerateChoicesForDocuments(initialUniqueChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 1);

			ImportDataSource<object[]> initialDataSource = this.GenerateImportDataSourceForSingleField(documentIdsList, choiceName, initialChoicesList);
			ImportTestJobResult firstImportResult = this.JobExecutionContext.Execute(initialDataSource);
			this.ThenTheImportJobIsSuccessful(firstImportResult, NumberOfDocumentsToImport);

			// prepare data for import under test
			string[] uniqueChoices = { "e", "f", "g", "h" };
			IList<string[]> choicesList = GenerateChoicesForDocuments(uniqueChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 1).ToList();

			ImportDataSource<object[]> dataSource = this.GenerateImportDataSourceForSingleField(documentIdsList, choiceName, choicesList);

			IEnumerable<DocumentChoicesDto> expectedChoiceToDocumentMapping = documentIdsList
				.Zip(choicesList, (id, choices) => new DocumentChoicesDto(id, choices));

			// ACT
			ImportTestJobResult secondImportResult = this.JobExecutionContext.Execute(dataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(secondImportResult, NumberOfDocumentsToImport);
			await this.ThenTheChoiceFieldHasExpectedValues(choiceName, expectedChoiceToDocumentMapping).ConfigureAwait(false);
		}

		[IdentifiedTest("51c1ea5f-ffd3-43d4-b074-cd3db72c9301")]
		public Task ShouldReplaceExistingMultiChoicesValues() => this.MultiChoiceTestAsync(
			importOverlayBehavior: OverlayBehavior.ReplaceAll,
			fieldOverlayBehavior: Services.Interfaces.Field.Models.OverlayBehavior.MergeValues,
			expectedShouldMergeValues: false);

		[IdentifiedTest("507a2802-7003-442f-8715-a3d1539410a8")]
		public Task ShouldMergeExistingMultiChoicesWithNewValues()
			=> this.MultiChoiceTestAsync(OverlayBehavior.MergeAll, Services.Interfaces.Field.Models.OverlayBehavior.ReplaceValues, expectedShouldMergeValues: true);

		[IdentifiedTestCase("ee4acebb-e5bd-4d70-a7b1-8474258972e6", Services.Interfaces.Field.Models.OverlayBehavior.MergeValues, true)]
		[IdentifiedTestCase("67aa07ba-12ea-4a48-b615-3f767114a0d1", Services.Interfaces.Field.Models.OverlayBehavior.ReplaceValues, false)]
		public Task ShouldUseDefaultMergeBehaviorForMultiChoices(Services.Interfaces.Field.Models.OverlayBehavior fieldOverlayBehavior, bool expectedShouldMergeValues)
			=> this.MultiChoiceTestAsync(OverlayBehavior.UseRelativityDefaults, fieldOverlayBehavior, expectedShouldMergeValues);

		[IdentifiedTest("d3809802-e166-49a2-a30b-30b0cb53eef2")]
		public async Task ShouldImportSingleAndMultiChoiceInOneJob()
		{
			// ARRANGE
			this.InitializeExecutionContext(OverlayBehavior.MergeAll);

			string singleChoiceFieldName = "SINGLE_CHOICE_FIELD";
			await this.CreateSingleChoiceFieldAsync(singleChoiceFieldName).ConfigureAwait(false);

			string multiChoiceFieldName = "MULTI_CHOICE_FIELD";
			await this.CreateMultiChoiceFieldAsync(multiChoiceFieldName).ConfigureAwait(false);

			List<string> documentIdsList = GenerateIdsForDocuments(NumberOfDocumentsToImport);

			// import initial data to workspace
			string[] initialUniqueSingleChoices = { "a", "b", "c", "d" };
			IEnumerable<string[]> initialSingleChoicesList = GenerateChoicesForDocuments(initialUniqueSingleChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 1);

			string[] initialUniqueMultiChoices = { "1", "2", "3", "4" };
			IEnumerable<string[]> initialMultiChoicesList = GenerateChoicesForDocuments(initialUniqueMultiChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 3).ToList();

			ImportDataSource<object[]> initialDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, documentIdsList)
				.AddField(singleChoiceFieldName, this.ConvertToChoicesListToImportApiFormat(initialSingleChoicesList))
				.AddField(multiChoiceFieldName, this.ConvertToChoicesListToImportApiFormat(initialMultiChoicesList))
				.Build();

			ImportTestJobResult firstImportResult = this.JobExecutionContext.Execute(initialDataSource);
			this.ThenTheImportJobIsSuccessful(firstImportResult, NumberOfDocumentsToImport);

			// prepare data for import under test
			string[] uniqueSingleChoices = { "e", "f", "g", "h" };
			IList<string[]> singleChoicesList = GenerateChoicesForDocuments(uniqueSingleChoices, NumberOfDocumentsToImport, 1).ToList();

			string[] uniqueMultiChoices = { "5", "6", "7", "8" };
			IList<string[]> multiChoicesList = GenerateChoicesForDocuments(uniqueMultiChoices, NumberOfDocumentsToImport, 3).ToList();

			IEnumerable<DocumentChoicesDto> expectedSingleChoiceToDocumentMapping = documentIdsList
				.Zip(singleChoicesList, (id, choices) => new DocumentChoicesDto(id, choices));

			IEnumerable<DocumentChoicesDto> expectedMultiChoiceToDocumentMapping = documentIdsList
				.Zip(
					multiChoicesList,
					(id, choices) => new DocumentChoicesDto(id, choices))
				.Zip(
					initialMultiChoicesList,
					(expectedChoices, initialChoicesNames) => expectedChoices.AddChoicesNames(initialChoicesNames));

			ImportDataSource<object[]> dataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, documentIdsList)
				.AddField(singleChoiceFieldName, this.ConvertToChoicesListToImportApiFormat(singleChoicesList))
				.AddField(multiChoiceFieldName, this.ConvertToChoicesListToImportApiFormat(multiChoicesList))
				.Build();

			// ACT
			ImportTestJobResult secondImportResult = this.JobExecutionContext.Execute(dataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(secondImportResult, NumberOfDocumentsToImport);
			await this.ThenTheChoiceFieldHasExpectedValues(singleChoiceFieldName, expectedSingleChoiceToDocumentMapping).ConfigureAwait(false);
			await this.ThenTheChoiceFieldHasExpectedValues(multiChoiceFieldName, expectedMultiChoiceToDocumentMapping).ConfigureAwait(false);
		}

		private static List<string> GenerateIdsForDocuments(int numberOfDocuments)
		{
			var documentsIds = new IdentifierValueSource("DOC");
			return documentsIds.CreateValuesEnumerator()
				.Cast<string>()
				.Take(numberOfDocuments)
				.ToList();
		}

		private static IEnumerable<string[]> GenerateChoicesForDocuments(string[] uniqueValues, int numberOfDocuments, int maxNumberOfMultiValues)
		{
			var randomGenerator = new Random(42);

			for (int documentIndex = 0; documentIndex < numberOfDocuments; documentIndex++)
			{
				int numberOfMultiValues = randomGenerator.Next(1, maxNumberOfMultiValues + 1);
				string[] choicesForDocument = Enumerable.Range(0, numberOfMultiValues)
					.Select(x => randomGenerator.NextElement(uniqueValues))
					.Distinct()
					.ToArray();

				yield return choicesForDocument;
			}
		}

		private async Task MultiChoiceTestAsync(OverlayBehavior importOverlayBehavior, Services.Interfaces.Field.Models.OverlayBehavior fieldOverlayBehavior, bool expectedShouldMergeValues)
		{
			// ARRANGE
			this.InitializeExecutionContext(importOverlayBehavior);

			string choiceName = "MULTI_CHOICE_FIELD";
			await this.CreateMultiChoiceFieldAsync(choiceName, fieldOverlayBehavior).ConfigureAwait(false);

			List<string> documentIdsList = GenerateIdsForDocuments(NumberOfDocumentsToImport);

			// import initial data to workspace
			string[] initialUniqueChoices = { "a", "b", "c", "d" };
			IList<string[]> initialChoicesList = GenerateChoicesForDocuments(initialUniqueChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 3).ToList();

			var initialDataSource = this.GenerateImportDataSourceForSingleField(documentIdsList, choiceName, initialChoicesList);
			ImportTestJobResult firstImportResult = this.JobExecutionContext.Execute(initialDataSource);
			this.ThenTheImportJobIsSuccessful(firstImportResult, NumberOfDocumentsToImport);

			// prepare data for import under test
			string[] uniqueChoices = { "e", "f", "g", "h" };
			IList<string[]> choicesList = GenerateChoicesForDocuments(uniqueChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 3).ToList();

			IEnumerable<DocumentChoicesDto> expectedChoiceToDocumentMapping = documentIdsList.Zip(
						choicesList,
						(id, choices) => new DocumentChoicesDto(id, choices));

			if (expectedShouldMergeValues)
			{
				expectedChoiceToDocumentMapping = expectedChoiceToDocumentMapping.Zip(
					initialChoicesList,
					(expectedChoices, initialChoicesNames) => expectedChoices.AddChoicesNames(initialChoicesNames));
			}

			var dataSource = this.GenerateImportDataSourceForSingleField(documentIdsList, choiceName, choicesList);

			// ACT
			ImportTestJobResult secondImportResult = this.JobExecutionContext.Execute(dataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(secondImportResult, NumberOfDocumentsToImport);
			await this.ThenTheChoiceFieldHasExpectedValues(choiceName, expectedChoiceToDocumentMapping).ConfigureAwait(false);
		}

		private void InitializeExecutionContext(OverlayBehavior overlayBehavior)
		{
			var settings = NativeImportSettingsBuilder
				.New()
				.WithDefaultSettings()
				.WithOverwriteMode(OverwriteModeEnum.AppendOverlay)
				.WithFieldOverlayMode(overlayBehavior)
				.Build();

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);
		}

		private async Task CreateMultiChoiceFieldAsync(
			string fieldName,
			Services.Interfaces.Field.Models.OverlayBehavior fieldOverlayBehavior = Services.Interfaces.Field.Models.OverlayBehavior.MergeValues)
		{
			var request = new MultipleChoiceFieldRequest
			{
				Name = fieldName,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = (int)ArtifactType.Document },
				OpenToAssociations = false,
				HasUnicode = true,
				OverlayBehavior = fieldOverlayBehavior,
			};

			int choiceFieldId = await FieldHelper.CreateFieldAsync(this.TestParameters, request).ConfigureAwait(false);
			this.createdFieldsIds.Add(choiceFieldId);
		}

		private async Task CreateSingleChoiceFieldAsync(string fieldName)
		{
			int singleChoiceFieldId = await FieldHelper
										  .CreateSingleChoiceFieldAsync(this.TestParameters, fieldName, (int)ArtifactType.Document, isOpenToAssociations: false)
										  .ConfigureAwait(false);
			this.createdFieldsIds.Add(singleChoiceFieldId);
		}

		private ImportDataSource<object[]> GenerateImportDataSourceForSingleField(
			IEnumerable<string> documentIdsList,
			string choiceName,
			IEnumerable<string[]> choicesValuesList)
		{
			return ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, documentIdsList)
				.AddField(choiceName, this.ConvertToChoicesListToImportApiFormat(choicesValuesList))
				.Build();
		}

		private IEnumerable<string> ConvertToChoicesListToImportApiFormat(IEnumerable<string[]> choicesForDocuments)
			=> choicesForDocuments.Select(choicesForDocument => string.Join(this.multiValueDelimiter, choicesForDocument));

		private async Task<Dictionary<string, List<string>>> GetChoiceFieldValuesForDocumentsAsync(string fieldName)
		{
			var request = new QueryRequest
			{
				ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Document },
				Fields = new[]
				{
					new FieldRef { Name = WellKnownFields.ControlNumber },
					new FieldRef { Name = fieldName },
				},
			};
			QueryResultSlim result;
			using (var objectManager = ServiceHelper.GetServiceProxy<IObjectManager>(this.TestParameters))
			{
				result = await objectManager.QuerySlimAsync(this.TestParameters.WorkspaceId, request, 0, NumberOfDocumentsToImport).ConfigureAwait(false);
			}

			var fieldsValuesForDocuments = new Dictionary<string, List<string>>();
			foreach (RelativityObjectSlim document in result.Objects)
			{
				string documentId = document.Values.First().ToString();
				fieldsValuesForDocuments[documentId] = new List<string>();

				object choiceFieldValue = document.Values.Skip(1).First();
				if (choiceFieldValue is Choice choice)
				{
					fieldsValuesForDocuments[documentId].Add(choice.Name);
				}
				else if (choiceFieldValue is IList<Choice> choices)
				{
					fieldsValuesForDocuments[documentId].AddRange(choices.Select(x => x.Name));
				}
			}

			return fieldsValuesForDocuments;
		}

		private async Task ThenTheChoiceFieldHasExpectedValues(string choiceName, IEnumerable<DocumentChoicesDto> expectedChoiceToDocumentMapping)
		{
			Dictionary<string, List<string>> actualChoiceToDocumentMapping = await this.GetChoiceFieldValuesForDocumentsAsync(choiceName).ConfigureAwait(false);

			foreach (var expectedMapping in expectedChoiceToDocumentMapping)
			{
				Assert.That(actualChoiceToDocumentMapping, Contains.Key(expectedMapping.ControlNumber));

				List<string> actualChoiceValuesForDocument = actualChoiceToDocumentMapping[expectedMapping.ControlNumber];
				CollectionAssert.AreEquivalent(expectedMapping.ChoicesNames, actualChoiceValuesForDocument);
			}
		}

		private class DocumentChoicesDto
		{
			public DocumentChoicesDto(string controlNumber, IEnumerable<string> choicesNames)
			{
				this.ControlNumber = controlNumber;
				this.ChoicesNames = choicesNames.ToList();
			}

			public string ControlNumber { get; }

			public List<string> ChoicesNames { get; }

			public DocumentChoicesDto AddChoicesNames(IEnumerable<string> choicesNames)
			{
				this.ChoicesNames.AddRange(choicesNames);
				return this;
			}
		}
	}
}
