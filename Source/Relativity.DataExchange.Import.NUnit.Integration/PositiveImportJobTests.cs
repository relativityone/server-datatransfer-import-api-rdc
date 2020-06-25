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
	using System.Collections.Generic;
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

		private const RelativityVersion MinSupportedVersion = RelativityVersion.Indigo;
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

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("f808845d-c8c9-454b-9d84-51d84be70bd1")]
		[Test]
		[Pairwise]
		public void ShouldImportTheFiles(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(true, false)] bool disableNativeLocationValidation,
			[Values(true, false)] bool disableNativeValidation)
		{
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.DefaultSettings(artifactTypeId);

			// ARRANGE
			ForceClient(client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			const int NumberOfFilesToImport = 5;
			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfFilesToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfFilesToImport));
			ThenTheJobCompletedInCorrectTransferMode(results, client);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, artifactTypeId, fields: new[] { WellKnownFields.ControlNumber });
			Assert.That(relativityObjects.Count, Is.EqualTo(NumberOfFilesToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.NotInCompatibility)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db0852938789")]
		public void ShouldImportFolders()
		{
			// ARRANGE
			const TapiClient Client = TapiClient.Direct;
			ForceClient(Client);
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);

			Settings settings = NativeImportSettingsProvider.DefaultSettings();
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
			ThenTheJobCompletedInCorrectTransferMode(results, Client);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("3723e0e9-2ce1-472b-b655-8fbffb515c1a")]
		public void ShouldAppendOverlayDocumentsAndMoveToNewFolders()
		{
			// ARRANGE
			const TapiClient Client = TapiClient.Direct;
			ForceClient(Client);
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);

			Settings settings = NativeImportSettingsProvider.DefaultSettings();
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			settings.MoveDocumentsInAppendOverlayMode = true;
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			int numberOfDocumentsToImport = TestData.SampleDocFiles.Count();
			IEnumerable<FolderImportDto> importData =
				TestData.SampleDocFiles.Select(p => new FolderImportDto(Path.GetFileName(p), @"\aaa \cc"));

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(result, numberOfDocumentsToImport);
			Assert.That(result.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));
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
			Settings settings = NativeImportSettingsProvider.DefaultSettings(artifactTypeId);

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
				controlNumber,
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
			Settings settings = NativeImportSettingsProvider.DefaultSettings(artifactTypeId);
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
			Settings settings = NativeImportSettingsProvider.DefaultSettings(artifactTypeId);

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

			await this.objectsValidator.ValidateObjectFieldsValuesWithExpected(
				controlNumber,
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
			Settings settings = NativeImportSettingsProvider.DefaultSettings(artifactTypeId);

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

			await this.objectsValidator.ValidateObjectFieldsValuesWithExpected(
				controlNumber,
				fieldsAndValuesToValidate,
				multiValueDelimiter,
				artifactTypeId).ConfigureAwait(false);
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
				multiObjectArtifactTypeId,
				artifactTypeId).ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(
				this.TestParameters,
				MultiObjectFieldName2,
				multiObjectArtifactTypeId,
				artifactTypeId).ConfigureAwait(false);

			int singleObjectArtifactTypeId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"Single object type for artifactTypeId {artifactTypeId}")
				                         .ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(
				this.TestParameters,
				SingleObjectFieldName,
				singleObjectArtifactTypeId,
				artifactTypeId).ConfigureAwait(false);
		}
	}
}