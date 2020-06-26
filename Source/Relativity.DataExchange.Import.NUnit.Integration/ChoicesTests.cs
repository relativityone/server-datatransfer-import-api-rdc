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
	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Testing.Identification;

	using OverlayBehavior = kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior;

	[TestFixture]
	public class ChoicesTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const int NumberOfDocumentsToImport = 10;
		private const RelativityVersion MinSupportedVersion = RelativityVersion.Goatsbeard;
		private readonly string multiValueDelimiter = SettingsConstants.DefaultMultiValueDelimiter.ToString();

		private bool testsSkipped = false;

		private int originalImportBatchSize;
		private List<int> createdFieldsIds = new List<int>();

		private ChoicesValidator choicesValidator;

		[OneTimeSetUp]
		public Task OneTimeSetUpAsync()
		{
			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(this.TestParameters, MinSupportedVersion);
			if (!testsSkipped)
			{
				this.originalImportBatchSize = AppSettings.Instance.ImportBatchSize;
				AppSettings.Instance.ImportBatchSize = 4;

				choicesValidator = new ChoicesValidator(this.TestParameters);

				return RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document);
			}

			return Task.CompletedTask;
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (!testsSkipped)
			{
				AppSettings.Instance.ImportBatchSize = this.originalImportBatchSize;
			}
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			if (!testsSkipped)
			{
				var deleteDocumentsTask = RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document);

				foreach (var createdFieldId in this.createdFieldsIds)
				{
					await FieldHelper.DeleteFieldAsync(this.TestParameters, createdFieldId).ConfigureAwait(false);
				}

				await deleteDocumentsTask.ConfigureAwait(false);

				this.createdFieldsIds = new List<int>();
			}
		}

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTestCase("0e72e6f1-6c53-45ca-9d5d-d9ccb46f2195", new[] { "a", "b", "c", "d" }, OverlayBehavior.MergeAll)]
		[IdentifiedTestCase("5fa9f2a6-3a8a-47c3-aeac-b558f3ad0c02", new[] { "a", "b", "c", "d" }, OverlayBehavior.ReplaceAll)]
		[IdentifiedTestCase("dc3a8ee9-dbcc-4444-b480-4c2912f4ff1f", new[] { "a", "b", "c", "d" }, OverlayBehavior.UseRelativityDefaults)]
		[IdentifiedTestCase("1008c5c7-fd88-4315-9c02-5fe0413dace6", new[] { "a", "" }, OverlayBehavior.UseRelativityDefaults)]
		public Task ShouldReplaceExistingValuesForSingleChoiceFieldAsync(string[] initialUniqueChoices, OverlayBehavior overlayBehavior)
			=> this.SingleChoiceTestAsync(overlayBehavior, initialUniqueChoices, uniqueChoices: new[] { "e", "f", "g", "h" });

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("14c4060d-51da-4187-b41a-3de675684239")]
		public Task ShouldUnlinkExistingValuesForSingleChoiceFieldAsync()
			=> this.SingleChoiceTestAsync(OverlayBehavior.ReplaceAll, initialUniqueChoices: new[] { "a", "b", "c", "d" }, uniqueChoices: new[] { string.Empty });

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTestCase("51c1ea5f-ffd3-43d4-b074-cd3db72c9301", new[] { "a", "b", "c", "d" }, Services.Interfaces.Field.Models.OverlayBehavior.MergeValues)]
		[IdentifiedTestCase("2b265e75-c753-485d-bf9d-fe5e4568e752", new[] { "a", "" }, Services.Interfaces.Field.Models.OverlayBehavior.ReplaceValues)]
		public Task ShouldReplaceExistingMultiChoicesValues(string[] initialUniqueChoices, Services.Interfaces.Field.Models.OverlayBehavior fieldOverlayBehavior) => this.MultiChoiceTestAsync(
			importOverlayBehavior: OverlayBehavior.ReplaceAll,
			fieldOverlayBehavior,
			initialUniqueChoices,
			uniqueChoices: new[] { "e", "f", "g", "h" },
			expectedShouldMergeValues: false);

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTestCase("51c1ea5f-ffd3-43d4-b074-cd3db72c9301", new[] { "a", "b", "c", "d" }, new[] { "" })]
		[IdentifiedTestCase("a8b602ba-c305-4185-8c31-b901388a15c4", new[] { "a", "b", "c", "d" }, new[] { "a", "" })]
		[IdentifiedTestCase("72dbd4e1-48d4-431e-89d9-5107cf096b85", new[] { "a", "" }, new[] { "e", "" })]
		public Task ShouldUnlinkExistingMultiChoicesValuesWhenNewValueInEmpty(string[] initialUniqueChoices, string[] uniqueChoices) => this.MultiChoiceTestAsync(
			importOverlayBehavior: OverlayBehavior.ReplaceAll,
			fieldOverlayBehavior: Services.Interfaces.Field.Models.OverlayBehavior.ReplaceValues,
			initialUniqueChoices,
			uniqueChoices,
			expectedShouldMergeValues: false);

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTestCase("507a2802-7003-442f-8715-a3d1539410a8", new[] { "a", "b", "c", "d" }, Services.Interfaces.Field.Models.OverlayBehavior.ReplaceValues)]
		[IdentifiedTestCase("9c5e6380-cdf9-44c5-b730-148b22043ce9", new[] { "a", "" }, Services.Interfaces.Field.Models.OverlayBehavior.MergeValues)]
		public Task ShouldMergeExistingMultiChoicesWithNewValues(string[] initialUniqueChoices, Services.Interfaces.Field.Models.OverlayBehavior fieldOverlayBehavior)
			=> this.MultiChoiceTestAsync(
				importOverlayBehavior: OverlayBehavior.MergeAll,
				fieldOverlayBehavior,
				initialUniqueChoices,
				uniqueChoices: new[] { "e", "f", "g", "h" },
				expectedShouldMergeValues: true);

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTestCase("ee4acebb-e5bd-4d70-a7b1-8474258972e6", Services.Interfaces.Field.Models.OverlayBehavior.MergeValues, true)]
		[IdentifiedTestCase("67aa07ba-12ea-4a48-b615-3f767114a0d1", Services.Interfaces.Field.Models.OverlayBehavior.ReplaceValues, false)]
		public Task ShouldUseDefaultMergeBehaviorForMultiChoices(Services.Interfaces.Field.Models.OverlayBehavior fieldOverlayBehavior, bool expectedShouldMergeValues)
			=> this.MultiChoiceTestAsync(
				importOverlayBehavior: OverlayBehavior.UseRelativityDefaults,
				fieldOverlayBehavior,
				initialUniqueChoices: new[] { "a", "b", "c", "d" },
				uniqueChoices: new[] { "e", "f", "g", "h" },
				expectedShouldMergeValues);

		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTestCase("d3809802-e166-49a2-a30b-30b0cb53eef2", NumberOfDocumentsToImport)]
		[IdentifiedTestCase("9c5952f2-5098-4ad4-876c-79109a369ae2", NumberOfDocumentsToImport / 2)]
		public async Task ShouldImportSingleAndMultiChoiceInOneJob(int numberOfDocumentToImportInImportUnderTest)
		{
			// ARRANGE
			this.InitializeExecutionContext(OverlayBehavior.MergeAll, OverwriteModeEnum.Append);

			string singleChoiceFieldName = "SINGLE_CHOICE_FIELD";
			await this.CreateSingleChoiceFieldAsync(singleChoiceFieldName).ConfigureAwait(false);

			string multiChoiceFieldName = "MULTI_CHOICE_FIELD";
			await this.CreateMultiChoiceFieldAsync(multiChoiceFieldName).ConfigureAwait(false);

			List<string> documentIdsList = GenerateIdsForDocuments(NumberOfDocumentsToImport);

			// import initial data to workspace
			string[] initialUniqueSingleChoices = { "a", "b", "c", "d" };
			IEnumerable<string[]> initialSingleChoicesList = GenerateChoicesForDocuments(initialUniqueSingleChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 1).ToList();

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
			this.InitializeExecutionContext(OverlayBehavior.MergeAll, OverwriteModeEnum.AppendOverlay);
			string[] uniqueSingleChoices = { "e", "f", "g", "h" };
			IList<string[]> singleChoicesList = GenerateChoicesForDocuments(uniqueSingleChoices, numberOfDocumentToImportInImportUnderTest, 1).ToList();

			string[] uniqueMultiChoices = { "5", "6", "7", "8" };
			IList<string[]> multiChoicesList = GenerateChoicesForDocuments(uniqueMultiChoices, numberOfDocumentToImportInImportUnderTest, 3).ToList();

			IEnumerable<DocumentChoicesDto> expectedSingleChoiceToDocumentMappingForOverlaidDocuments = documentIdsList
				.Take(numberOfDocumentToImportInImportUnderTest)
				.Zip(singleChoicesList, (id, choices) => new DocumentChoicesDto(id, choices));

			IEnumerable<DocumentChoicesDto> expectedSingleChoiceToDocumentMappingForNotModifiedDocuments = documentIdsList
				.Skip(numberOfDocumentToImportInImportUnderTest)
				.Zip(initialSingleChoicesList.Skip(numberOfDocumentToImportInImportUnderTest), (id, choices) => new DocumentChoicesDto(id, choices));

			IEnumerable<DocumentChoicesDto> expectedMultiChoiceToDocumentMappingForOverlaidDocuments = documentIdsList
				.Take(numberOfDocumentToImportInImportUnderTest)
				.Zip(
					multiChoicesList,
					(id, choices) => new DocumentChoicesDto(id, choices))
				.Zip(
					initialMultiChoicesList,
					(expectedChoices, initialChoicesNames) => expectedChoices.AddChoicesNames(initialChoicesNames));

			IEnumerable<DocumentChoicesDto> expectedMultiChoiceToDocumentMappingForNotModifiedDocuments = documentIdsList
				.Skip(numberOfDocumentToImportInImportUnderTest)
				.Zip(
					initialMultiChoicesList.Skip(numberOfDocumentToImportInImportUnderTest),
					(id, choices) => new DocumentChoicesDto(id, choices));

			ImportDataSource<object[]> dataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, documentIdsList.Take(numberOfDocumentToImportInImportUnderTest))
				.AddField(singleChoiceFieldName, this.ConvertToChoicesListToImportApiFormat(singleChoicesList))
				.AddField(multiChoiceFieldName, this.ConvertToChoicesListToImportApiFormat(multiChoicesList))
				.Build();

			// ACT
			ImportTestJobResult secondImportResult = this.JobExecutionContext.Execute(dataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(secondImportResult, numberOfDocumentToImportInImportUnderTest);

			// overlaid documents
			await this.choicesValidator.ThenTheChoiceFieldHasExpectedValues(singleChoiceFieldName, WellKnownFields.ControlNumber, expectedSingleChoiceToDocumentMappingForOverlaidDocuments, (int)ArtifactType.Document).ConfigureAwait(false);
			await this.choicesValidator.ThenTheChoiceFieldHasExpectedValues(multiChoiceFieldName, WellKnownFields.ControlNumber, expectedMultiChoiceToDocumentMappingForOverlaidDocuments, (int)ArtifactType.Document).ConfigureAwait(false);

			// not modified documents
			await this.choicesValidator.ThenTheChoiceFieldHasExpectedValues(singleChoiceFieldName, WellKnownFields.ControlNumber, expectedSingleChoiceToDocumentMappingForNotModifiedDocuments, (int)ArtifactType.Document).ConfigureAwait(false);
			await this.choicesValidator.ThenTheChoiceFieldHasExpectedValues(multiChoiceFieldName, WellKnownFields.ControlNumber, expectedMultiChoiceToDocumentMappingForNotModifiedDocuments, (int)ArtifactType.Document).ConfigureAwait(false);
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

		private async Task SingleChoiceTestAsync(OverlayBehavior overlayBehavior, string[] initialUniqueChoices, string[] uniqueChoices)
		{
			// ARRANGE
			this.InitializeExecutionContext(overlayBehavior, OverwriteModeEnum.Append);

			string choiceName = "SINGLE_CHOICE_FIELD";
			await this.CreateSingleChoiceFieldAsync(choiceName).ConfigureAwait(false);

			List<string> documentIdsList = GenerateIdsForDocuments(NumberOfDocumentsToImport);

			// import initial data to workspace
			IEnumerable<string[]> initialChoicesList = GenerateChoicesForDocuments(initialUniqueChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 1);

			ImportDataSource<object[]> initialDataSource = this.GenerateImportDataSourceForSingleField(documentIdsList, choiceName, initialChoicesList);
			ImportTestJobResult firstImportResult = this.JobExecutionContext.Execute(initialDataSource);
			this.ThenTheImportJobIsSuccessful(firstImportResult, NumberOfDocumentsToImport);

			// prepare data for import under test
			this.InitializeExecutionContext(overlayBehavior, OverwriteModeEnum.AppendOverlay);
			IList<string[]> choicesList = GenerateChoicesForDocuments(uniqueChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 1).ToList();

			ImportDataSource<object[]> dataSource = this.GenerateImportDataSourceForSingleField(documentIdsList, choiceName, choicesList);

			IEnumerable<DocumentChoicesDto> expectedChoiceToDocumentMapping = documentIdsList
				.Zip(choicesList, (id, choices) => new DocumentChoicesDto(id, choices));

			// ACT
			ImportTestJobResult secondImportResult = this.JobExecutionContext.Execute(dataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(secondImportResult, NumberOfDocumentsToImport);
			await this.choicesValidator.ThenTheChoiceFieldHasExpectedValues(choiceName, WellKnownFields.ControlNumber, expectedChoiceToDocumentMapping, (int)ArtifactType.Document).ConfigureAwait(false);
		}

		private async Task MultiChoiceTestAsync(
			OverlayBehavior importOverlayBehavior,
			Services.Interfaces.Field.Models.OverlayBehavior fieldOverlayBehavior,
			string[] initialUniqueChoices,
			string[] uniqueChoices,
			bool expectedShouldMergeValues)
		{
			// ARRANGE
			this.InitializeExecutionContext(importOverlayBehavior, OverwriteModeEnum.Append);

			string choiceName = "MULTI_CHOICE_FIELD";
			await this.CreateMultiChoiceFieldAsync(choiceName, fieldOverlayBehavior).ConfigureAwait(false);

			List<string> documentIdsList = GenerateIdsForDocuments(NumberOfDocumentsToImport);

			// import initial data to workspace
			IList<string[]> initialChoicesList = GenerateChoicesForDocuments(initialUniqueChoices, NumberOfDocumentsToImport, maxNumberOfMultiValues: 3).ToList();

			var initialDataSource = this.GenerateImportDataSourceForSingleField(documentIdsList, choiceName, initialChoicesList);
			ImportTestJobResult firstImportResult = this.JobExecutionContext.Execute(initialDataSource);
			this.ThenTheImportJobIsSuccessful(firstImportResult, NumberOfDocumentsToImport);

			// prepare data for import under test
			this.InitializeExecutionContext(importOverlayBehavior, OverwriteModeEnum.AppendOverlay);
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
			await this.choicesValidator.ThenTheChoiceFieldHasExpectedValues(choiceName, WellKnownFields.ControlNumber, expectedChoiceToDocumentMapping, (int)ArtifactType.Document).ConfigureAwait(false);
		}

		private void InitializeExecutionContext(OverlayBehavior overlayBehavior, OverwriteModeEnum overwriteMode)
		{
			var settings = NativeImportSettingsBuilder
				.New()
				.WithDefaultSettings()
				.WithOverwriteMode(overwriteMode)
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
	}
}
