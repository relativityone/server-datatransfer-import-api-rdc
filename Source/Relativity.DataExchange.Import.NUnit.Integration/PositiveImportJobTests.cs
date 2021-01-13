// -----------------------------------------------------------------------------------------------------
// <copyright file="PositiveImportJobTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Dynamic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;
	using kCura.Relativity.DataReaderClient;
	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImportJobTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const string SingleChoiceFieldName = "SINGLE_CHOICE_FIELD";
		private const string MultiChoiceFieldName1 = "MULTI_CHOICE_FIELD_1";
		private const string MultiChoiceFieldName2 = "MULTI_CHOICE_FIELD_2";
		private const string MultiChoiceFieldNameNotExisting = "MULTI_CHOICE_FIELD_NOT_EXISTING";

		private const string SingleObjectFieldName = "SINGLE_OBJECT_FIELD";
		private const string MultiObjectFieldName1 = "MULTI_OBJECT_FIELD_1";
		private const string MultiObjectFieldName2 = "MULTI_OBJECT_FIELD_2";

		private const string SingleObjectDocFieldName = "SINGLE_OBJECT_DOC_FIELD";
		private const string MultiObjectDocFieldName = "MULTI_OBJECT_DOC_FIELD";

		private const RelativityVersion MinSupportedVersion = RelativityVersion.Foxglove;
		private bool testsSkipped = false;

		private int createdObjectArtifactTypeId = 0;

		private ObjectsValidator objectsValidator;

		private ChoicesValidator choicesValidator;

		[OneTimeSetUp]
		public async Task SetupObjectAsync()
		{
			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(
							   this.TestParameters,
							   MinSupportedVersion);
			if (!testsSkipped)
			{
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false); // Remove all Documents imported in AssemblySetup
				createdObjectArtifactTypeId = await this.CreateObjectInWorkspaceAsync().ConfigureAwait(false);
				await CreateChoiceFieldsAsync(this.createdObjectArtifactTypeId).ConfigureAwait(false);
				await CreateChoiceFieldsAsync((int)ArtifactTypeID.Document).ConfigureAwait(false);
				await CreateObjectFieldsAsync(this.createdObjectArtifactTypeId).ConfigureAwait(false);
				await CreateObjectFieldsAsync((int)ArtifactTypeID.Document).ConfigureAwait(false);
				await CreateAssociatedDocumentsFieldsAsync(this.createdObjectArtifactTypeId).ConfigureAwait(false);
				this.objectsValidator = new ObjectsValidator(this.TestParameters);
				this.choicesValidator = new ChoicesValidator(this.TestParameters);
			}
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			if (!testsSkipped)
			{
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, createdObjectArtifactTypeId).ConfigureAwait(false);
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
			}
		}

		[SuppressMessage("Microsoft.Maintainability", "CA1506", Justification = "It is just integration test")]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IgnoreIfVersionEqualTo(RelativityVersion.MayappleEAU)]
		[IdentifiedTest("f808845d-c8c9-454b-9d84-51d84be70bd1")]
		[Test]
		[Pairwise]
		public void ShouldImportTheFiles(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode,
			[Values(true, false)] bool disableNativeLocationValidation,
			[Values(true, false)] bool disableNativeValidation)
		{
			// There is a bug in Mayapple EAU that prevents to import file for object types
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.GetFileCopySettings(artifactTypeId);

			// ARRANGE
			ForceClient(client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			const int NumberOfFilesToImport = 5;
			DefaultImportDto[] importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport).ToArray();

			// Import initial data to workspace
			if (overwriteMode == OverwriteModeEnum.AppendOverlay || overwriteMode == OverwriteModeEnum.Overlay)
			{
				settings.OverwriteMode = OverwriteModeEnum.Append;
				this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

				DefaultImportDto[] initialData = overwriteMode == OverwriteModeEnum.AppendOverlay
					                                 ? importData.Take(importData.Length / 2).ToArray()
					                                 : importData;

				ImportTestJobResult initialImportResults = this.JobExecutionContext.Execute(initialData);
				this.ThenTheImportJobIsSuccessful(initialImportResults, initialData.Length);
			}

			// Prepare data for import under test
			settings.OverwriteMode = overwriteMode;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfFilesToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));

			// Progress count is doubled in case of the Object so we can't fix the assert on expected record count
			Assert.That(results.NumberOfCompletedRows, Is.GreaterThanOrEqualTo(NumberOfFilesToImport));
			ThenTheJobCompletedInCorrectTransferMode(results, client);

			string fileNameField = artifactTypeId == (int)ArtifactType.Document
									   ? WellKnownFields.HasNative
									   : WellKnownFields.FilePath;

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, artifactTypeId, fields: new[] { WellKnownFields.ControlNumber, fileNameField });
			Assert.That(relativityObjects.Count, Is.EqualTo(NumberOfFilesToImport));

			// Check the files are there - document has different field to check than object
			if (artifactTypeId == (int)ArtifactType.Document)
			{
				Assert.That(relativityObjects.All(item => (bool)item.FieldValues[1].Value));
			}
			else
			{
				Assert.That(relativityObjects.All(item => ((ExpandoObject)item.FieldValues[1].Value) != null));
			}
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.NotInCompatibility)]
		[IgnoreIfVersionLowerThan(RelativityVersion.Goatsbeard)]
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db0852938789")]
		public void ShouldImportFolders()
		{
			// ARRANGE
			const TapiClient Client = TapiClient.Direct;
			ForceClient(Client);
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();

			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			var randomFolderGenerator = RandomPathGenerator.GetFolderGenerator(
				numOfDifferentElements: 25,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 100);

			const int NumberOfDocumentsToImport = 2010;
			IEnumerable<FolderImportDto> importData = randomFolderGenerator.ToFolders(NumberOfDocumentsToImport)
				.Select((p, i) => new FolderImportDto($"{i}-{nameof(this.ShouldImportFolders)}", p));

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfDocumentsToImport));

			this.ValidateFieldsAfterImport(
				NumberOfDocumentsToImport,
				(int)ArtifactType.Document,
				new[] { WellKnownFields.ControlNumber, WellKnownFields.FolderName });

			ThenTheJobCompletedInCorrectTransferMode(results, Client);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("3723e0e9-2ce1-472b-b655-8fbffb515c1a")]
		public void ShouldAppendOverlayDocumentsAndMoveToNewFolders()
		{
			// ARRANGE
			const string DestinationFolderName = "cc";
			const TapiClient Client = TapiClient.Direct;
			ForceClient(Client);
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();

			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			settings.MoveDocumentsInAppendOverlayMode = true;
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			int numberOfDocumentsToImport = TestData.SampleDocFiles.Count();
			IEnumerable<FolderImportDto> importData =
				TestData.SampleDocFiles.Select(p => new FolderImportDto(Path.GetFileName(p), @"\aaa \" + DestinationFolderName));

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(result, numberOfDocumentsToImport);
			Assert.That(result.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));

			this.ValidateFieldsAfterImport(
				numberOfDocumentsToImport,
				(int)ArtifactType.Document,
				new[] { WellKnownFields.ControlNumber, WellKnownFields.FolderName });

			this.ValidateFilesWereMoved(DestinationFolderName, new[] { WellKnownFields.FolderName });

			ThenTheJobCompletedInCorrectTransferMode(result, Client);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("700bda86-6e9a-43c1-a69c-2a1972cba4f8")]
		public async Task ShouldImportDocumentWithChoices(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings(artifactTypeId);

			// Import initial data to workspace
			if (overwriteMode == OverwriteModeEnum.AppendOverlay || overwriteMode == OverwriteModeEnum.Overlay)
			{
				settings.OverwriteMode = OverwriteModeEnum.Append;
				this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

				int initialNumberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 201;
				int initialNumberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
				int initialNumberOfDocumentsToImport = initialNumberOfDocumentsToAppend + initialNumberOfDocumentsToOverlay;

				IEnumerable<string> controlNumberInitial = GetControlNumberEnumerable(overwriteMode, initialNumberOfDocumentsToAppend, $"{nameof(this.ShouldImportDocumentWithChoices)}{overwriteMode}").ToList();
				ImportDataSource<object[]> initialImportDataSource = ImportDataSourceBuilder.New().AddField(WellKnownFields.ControlNumber, controlNumberInitial).Build();
				ImportTestJobResult initialImportResults = this.JobExecutionContext.Execute(initialImportDataSource);
				this.ThenTheImportJobIsSuccessful(initialImportResults, initialNumberOfDocumentsToImport);
			}

			// Prepare data for import under test
			settings.OverwriteMode = overwriteMode;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			char multiValueDelimiter = settings.MultiValueDelimiter;
			char nestedValueDelimiter = settings.NestedValueDelimiter;

			int numberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 201;
			int numberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
			int numberOfDocumentsToImport = numberOfDocumentsToAppend + numberOfDocumentsToOverlay;

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(
				overwriteMode,
				numberOfDocumentsToAppend,
				$"{nameof(this.ShouldImportDocumentWithChoices)}{overwriteMode}").ToList();
			IEnumerable<string> singleChoiceField = RandomPathGenerator.GetChoiceGenerator(
				numOfDifferentElements: 100,
				maxElementLength: 200,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter).ToEnumerable().Take(numberOfDocumentsToImport).ToList();

			// Overlay replace.
			IEnumerable<string> multiChoiceField = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter).ToEnumerable(nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter).Take(numberOfDocumentsToImport).ToList();

			// Overlay append.
			IEnumerable<string> multiChoiceFieldNotExisting = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter).ToEnumerable(nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter).Take(numberOfDocumentsToImport).ToList();

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(SingleChoiceFieldName, singleChoiceField)
				.AddField(MultiChoiceFieldName1, multiChoiceField)
				.AddField(MultiChoiceFieldNameNotExisting, multiChoiceFieldNotExisting)
				.Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));

			ValidateJobMessagesContainsText(results, "Warning - Field MULTI_CHOICE_FIELD_NOT_EXISTING not exists in workspace");
			string[] fieldsToValidate = new[] { WellKnownFields.ControlNumber, SingleChoiceFieldName, MultiChoiceFieldName1, MultiChoiceFieldNameNotExisting };
			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, artifactTypeId, fieldsToValidate);
			Assert.That(relativityObjects.Count, Is.EqualTo(numberOfDocumentsToImport));
			ObjectsValidator.ThenObjectsFieldsAreImported(relativityObjects, fieldsToValidate: new[] { WellKnownFields.ControlNumber, MultiChoiceFieldName1, SingleChoiceFieldName });
			ObjectsValidator.ThenObjectsFieldsAreNotImported(relativityObjects, fieldsToValidate: new[] { MultiChoiceFieldNameNotExisting });

			Dictionary<string, IEnumerable<string>> fieldsAndValuesToValidate = new Dictionary<string, IEnumerable<string>>();
			fieldsAndValuesToValidate.Add(SingleChoiceFieldName, singleChoiceField);
			fieldsAndValuesToValidate.Add(MultiChoiceFieldName1, multiChoiceField);

			await this.choicesValidator.ValidateChoiceFieldsValuesWithExpected(
				new Tuple<string, IEnumerable<string>>(WellKnownFields.ControlNumber, controlNumber),
				fieldsAndValuesToValidate,
				multiValueDelimiter,
				nestedValueDelimiter,
				artifactTypeId).ConfigureAwait(false);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("e555aa7f-9976-4a74-87b4-577853209b57")]
		public void ShouldImportDocumentWithChoices2(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings(artifactTypeId);
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			DocumentWithChoicesImportDto[] importData =
				{
					new DocumentWithChoicesImportDto("100009", "qqqq2", "www;eee"),
					new DocumentWithChoicesImportDto("100010", "qqqq2", @"www;eee\rrr"),
					new DocumentWithChoicesImportDto("100011", "ÙJð\"ò2'=", "www;eee"),
					new DocumentWithChoicesImportDto("100012", "ÙJÐ\"Ò2'=", "ÙJð\"ò2'=;ÙJÐ\"Ò2'="),
					new DocumentWithChoicesImportDto("100013", " [IVÐ2ÃYQU>KPÔÇZXYLNÉÎPXÀØÛ", "www;eee"),
					new DocumentWithChoicesImportDto("100014", "[IVÐ2ÃYQU>KPÔÇZXYLNÉÎPXÀØÛ", "www;eee"),
				};

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, importData.Length);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(importData.Length));

			string[] fieldsToValidate = new[] { WellKnownFields.ControlNumber, SingleChoiceFieldName, MultiChoiceFieldName1 };
			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, artifactTypeId, fieldsToValidate);
			Assert.That(relativityObjects.Count, Is.EqualTo(importData.Length));
			ObjectsValidator.ThenObjectsFieldsAreImported(relativityObjects, fieldsToValidate);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("13dc1d17-4a2b-4b48-9015-b61e58bc5168")]
		public async Task ShouldImportDocumentWithObjects(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings(artifactTypeId);

			// Import initial data to workspace
			if (overwriteMode == OverwriteModeEnum.AppendOverlay || overwriteMode == OverwriteModeEnum.Overlay)
			{
				settings.OverwriteMode = OverwriteModeEnum.Append;
				this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

				int initialNumberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 2010;
				int initialNumberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
				int initialNumberOfDocumentsToImport = initialNumberOfDocumentsToAppend + initialNumberOfDocumentsToOverlay;

				IEnumerable<string> initialControlNumber = GetControlNumberEnumerable(overwriteMode, initialNumberOfDocumentsToAppend, $"{nameof(this.ShouldImportDocumentWithObjects)}{overwriteMode}");
				ImportDataSource<object[]> initialImportDataSource = ImportDataSourceBuilder.New().AddField(WellKnownFields.ControlNumber, initialControlNumber).Build();
				ImportTestJobResult initialImportResults = this.JobExecutionContext.Execute(initialImportDataSource);
				this.ThenTheImportJobIsSuccessful(initialImportResults, initialNumberOfDocumentsToImport);
			}

			// Prepare data for import under test
			settings.OverwriteMode = overwriteMode;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			char multiValueDelimiter = settings.MultiValueDelimiter;

			int numberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 2010;
			int numberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
			int numberOfDocumentsToImport = numberOfDocumentsToAppend + numberOfDocumentsToOverlay;

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(overwriteMode, numberOfDocumentsToAppend, $"{nameof(this.ShouldImportDocumentWithObjects)}{overwriteMode}").ToList();

			IEnumerable<string> singleObjectField = RandomPathGenerator.GetObjectGenerator(
				numOfDifferentElements: 100,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter).ToEnumerable().Take(numberOfDocumentsToImport).ToList();

			IEnumerable<string> multiObjectField1 = RandomPathGenerator.GetObjectGenerator(
					numOfDifferentElements: 300,
					maxElementLength: 255,
					numOfDifferentPaths: 100,
					maxPathDepth: 1,
					multiValueDelimiter: multiValueDelimiter).ToEnumerable()
				.RandomUniqueBatch(2, multiValueDelimiter)
				.Take(numberOfDocumentsToImport).ToList();

			IEnumerable<string> multiObjectField2 = RandomPathGenerator.GetObjectGenerator(
					numOfDifferentElements: 400,
					maxElementLength: 255,
					numOfDifferentPaths: 100,
					maxPathDepth: 1,
					multiValueDelimiter: multiValueDelimiter).ToEnumerable()
				.RandomUniqueBatch(5, multiValueDelimiter)
				.Take(numberOfDocumentsToImport).ToList();

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(SingleObjectFieldName, singleObjectField)
				.AddField(MultiObjectFieldName1, multiObjectField1)
				.AddField(MultiObjectFieldName2, multiObjectField2)
				.Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));

			Dictionary<string, IEnumerable<string>> fieldsAndValuesToValidate = new Dictionary<string, IEnumerable<string>>();
			fieldsAndValuesToValidate.Add(SingleObjectFieldName, singleObjectField);
			fieldsAndValuesToValidate.Add(MultiObjectFieldName1, multiObjectField1);
			fieldsAndValuesToValidate.Add(MultiObjectFieldName2, multiObjectField2);

			await this.objectsValidator.ValidateObjectFieldsValuesWithExpectedAsync(
				new Tuple<string, IEnumerable<string>>(WellKnownFields.ControlNumber, controlNumber),
				fieldsAndValuesToValidate,
				multiValueDelimiter,
				artifactTypeId).ConfigureAwait(false);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("bfd19759-3ef5-4752-84ab-268e9fb54e3d")]
		public async Task ShouldImportDocumentWithObjects2(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(OverwriteModeEnum.Append)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings(artifactTypeId);

			// Prepare data for import under test
			settings.OverwriteMode = overwriteMode;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			char multiValueDelimiter = settings.MultiValueDelimiter;

			int numberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 2;
			int numberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
			int numberOfDocumentsToImport = numberOfDocumentsToAppend + numberOfDocumentsToOverlay;

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(overwriteMode, numberOfDocumentsToAppend, $"{nameof(this.ShouldImportDocumentWithObjects)}{overwriteMode}").ToList();

			IEnumerable<string> multiObjectField = new List<string> { "UÁØ]7$(ÝÄ2H", " uáØ]7$(ÝÄ2H", "hÜ)ßêÏuNëiìÚÃOÒËetYÏÝÛþCLS4MÉI2dwÔ8má] {aLÑ<1dûxcêx3)ýëÎÝçrvzIÆÂWgCï5Dé'tÝNp*â0èX|éßGÿ[yèjÑúëi!8Ç.åÊA,b@ û~LÐZÄûÖ" };

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(MultiObjectFieldName1, multiObjectField).Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));

			Dictionary<string, IEnumerable<string>> fieldsAndValuesToValidate = new Dictionary<string, IEnumerable<string>>();
			fieldsAndValuesToValidate.Add(MultiObjectFieldName1, multiObjectField);

			await this.objectsValidator.ValidateObjectFieldsValuesWithExpectedAsync(
				new Tuple<string, IEnumerable<string>>(WellKnownFields.ControlNumber, controlNumber),
				fieldsAndValuesToValidate,
				multiValueDelimiter,
				artifactTypeId).ConfigureAwait(false);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IgnoreIfMassImportImprovementsToggleHasValue(isEnabled: false)]
		[IdentifiedTest("13dc1d17-4a2b-4b48-9015-b61e58bc5168")]
		public async Task ShouldImportObjectsWithAssociatedChildDocuments(
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.AppendOverlay, OverwriteModeEnum.Overlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			int recordCount = 2000;
			List<string> controlNumberValues = GetIdentifiersEnumerable(
				recordCount,
				$"{nameof(this.ShouldImportObjectsWithAssociatedChildDocuments)}{overwriteMode}").ToList();

			ImportDataSource<object[]> childDocumentDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberValues).Build();

			// First insert Documents to be later linked with objects - for now mass import does not allow to create new documents when importing objects
			Settings settings = NativeImportSettingsProvider.GetDefaultSettings((int)ArtifactType.Document);
			settings.OverwriteMode = OverwriteModeEnum.Append;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);
			ImportTestJobResult results = this.JobExecutionContext.Execute(childDocumentDataSource);

			this.ThenTheImportJobIsSuccessful(results, recordCount);

			// Next, for append/overlay and overlay test case, insert objects with only identifiers
			int artifactTypeId = GetArtifactTypeIdForTest(ArtifactType.ObjectType);
			settings = NativeImportSettingsProvider.GetDefaultSettings(artifactTypeId);

			List<string> objectIdentifiersValues = controlNumberValues.Select(ctrlNumber => $"Obj-{ctrlNumber}").ToList();

			if (overwriteMode != OverwriteModeEnum.Append)
			{
				ImportDataSource<object[]> initObjectsDataSource = ImportDataSourceBuilder.New()
					.AddField(WellKnownFields.ControlNumber, objectIdentifiersValues)
					.Build();

				this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

				results = this.JobExecutionContext.Execute(initObjectsDataSource);
				this.ThenTheImportJobIsSuccessful(results, recordCount);
			}

			char multiValueDelimiter = settings.MultiValueDelimiter;

			IEnumerable<string> shiftedControlNumberValues = controlNumberValues.Skip(1).Concat(new[] { controlNumberValues.First() }).ToList();

			IEnumerable<string> multiObjectDocFieldValues = controlNumberValues.Zip(shiftedControlNumberValues, (s, s1) => string.Join(multiValueDelimiter.ToString(), s, s1)).ToList();

			ImportDataSource<object[]> objectsDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, objectIdentifiersValues)
				.AddField(SingleObjectDocFieldName, controlNumberValues)
				.AddField(MultiObjectDocFieldName, multiObjectDocFieldValues)
				.Build();
			settings.OverwriteMode = overwriteMode;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT

			// Now import objects and link the documents
			results = this.JobExecutionContext.Execute(objectsDataSource);

			// ASSERT
			Dictionary<string, IEnumerable<string>> fieldsAndValuesToValidate =
				new Dictionary<string, IEnumerable<string>>
					{
						{ SingleObjectDocFieldName, controlNumberValues },
						{ MultiObjectDocFieldName, multiObjectDocFieldValues },
					};

			this.ThenTheImportJobIsSuccessful(results, recordCount);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(recordCount));

			await this.objectsValidator.ValidateObjectFieldsValuesWithExpectedAsync(
				new Tuple<string, IEnumerable<string>>(WellKnownFields.ControlNumber, objectIdentifiersValues),
				fieldsAndValuesToValidate,
				multiValueDelimiter,
				artifactTypeId).ConfigureAwait(false);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(RelativityVersion.Ninebark)]
		[IgnoreIfMassImportImprovementsToggleHasValue(isEnabled: false)]
		[IdentifiedTest("ad3bde44-9722-4f2c-8ef2-04453a639365")]
		[Description("This test verifies that we do not audit redundant information when appending a new document in an Append/Overlay mode.")]
		public async Task ShouldOnlyAuditCreationOfNewDocument()
		{
			// ARRANGE
			const int NumberOfDocumentsToAppend = 1;
			const string ExpectedAction = "Create";
			var testExecutionStartTime = DateTime.Now;

			var settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(
				OverwriteModeEnum.Append,
				NumberOfDocumentsToAppend,
				$"{nameof(this.ShouldOnlyAuditCreationOfNewDocument)}");

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber).Build();

			// ACT
			var results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfDocumentsToAppend);

			var audits = await AuditHelper.GetAuditActionsForSpecificObjectAsync(this.TestParameters, testExecutionStartTime, 20, this.GetDocumentArtifactId())
											 .ConfigureAwait(false);

			ThenTheAuditActionsAreCorrect(audits, ExpectedAction);
		}

		private static void ThenTheAuditActionsAreCorrect(IList<string> audits, string expectedAction)
		{
			foreach (var audit in audits)
			{
				Assert.That(audit.Equals(expectedAction));
			}
		}

		private int GetDocumentArtifactId()
		{
			IList<RelativityObject> listOfDocuments = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactType.Document, new string[] { WellKnownFields.ControlNumber });

			return listOfDocuments[0].ArtifactID;
		}

		private int GetArtifactTypeIdForTest(ArtifactType artifactType)
		{
			int artifactTypeId = artifactType == ArtifactType.Document
									 ? (int)ArtifactTypeID.Document
									 : this.createdObjectArtifactTypeId;
			return artifactTypeId;
		}

		private async Task CreateChoiceFieldsAsync(int artifactTypeId)
		{
			await FieldHelper.CreateSingleChoiceFieldAsync(
				this.TestParameters,
				SingleChoiceFieldName,
				artifactTypeId,
				false).ConfigureAwait(false);
			await FieldHelper.CreateMultiChoiceFieldAsync(
				this.TestParameters,
				MultiChoiceFieldName1,
				artifactTypeId,
				false).ConfigureAwait(false);
			await FieldHelper.CreateMultiChoiceFieldAsync(
				this.TestParameters,
				MultiChoiceFieldName2,
				artifactTypeId,
				false).ConfigureAwait(false);
		}

		private async Task CreateObjectFieldsAsync(int artifactTypeId)
		{
			int multiObjectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"Multi object type for artifactTypeId {artifactTypeId}")
									 .ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(
				this.TestParameters,
				MultiObjectFieldName1,
				objectArtifactTypeId: multiObjectArtifactTypeId,
				associativeObjectArtifactTypeId: artifactTypeId).ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(
				this.TestParameters,
				MultiObjectFieldName2,
				objectArtifactTypeId: multiObjectArtifactTypeId,
				associativeObjectArtifactTypeId: artifactTypeId).ConfigureAwait(false);

			int singleObjectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"Single object type for artifactTypeId {artifactTypeId}")
										 .ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(
				this.TestParameters,
				SingleObjectFieldName,
				objectArtifactTypeId: artifactTypeId,
				associativeObjectArtifactTypeId: singleObjectArtifactTypeId).ConfigureAwait(false);
		}

		private void ValidateFieldsAfterImport(int numberOfDocumentsToImport, int artifactTypeId, string[] fieldsToValidate)
		{
			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, artifactTypeId, fieldsToValidate);

			Assert.That(relativityObjects.Count, Is.EqualTo(numberOfDocumentsToImport));
			ObjectsValidator.ThenObjectsFieldsAreImported(relativityObjects, fieldsToValidate);
		}

		private void ValidateFilesWereMoved(string expectedFolderName, string[] actualFolderNames)
		{
			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactType.Document, actualFolderNames);

			foreach (RelativityObject relativityObject in relativityObjects)
			{
				Assert.AreEqual(expectedFolderName, relativityObject.FieldValues[0].Value.ToString(), "Files were not moved to an expected destination folder.");
			}
		}

		private async Task CreateAssociatedDocumentsFieldsAsync(int artifactTypeId)
		{
			if (artifactTypeId != (int)ArtifactType.Document)
			{
				await FieldHelper.CreateMultiObjectFieldAsync(
					this.TestParameters,
					MultiObjectDocFieldName,
					objectArtifactTypeId: artifactTypeId,
					associativeObjectArtifactTypeId: (int)ArtifactType.Document).ConfigureAwait(false);

				await FieldHelper.CreateSingleObjectFieldAsync(
					this.TestParameters,
					SingleObjectDocFieldName,
					objectArtifactTypeId: artifactTypeId,
					associativeObjectArtifactTypeId: (int)ArtifactType.Document).ConfigureAwait(false);
			}
		}
	}
}