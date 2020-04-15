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
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Interfaces.Field;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImportJobTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private int createdObjectArtifactTypeId = 0;

		private ObjectsValidator objectsValidator;

		private ChoicesValidator choicesValidator;

		[OneTimeSetUp]
		public async Task SetupObjectAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false); // Remove all Documents imported in AssemblySetup
			createdObjectArtifactTypeId = await this.CreateObjectInWorkspaceAsync().ConfigureAwait(false);
			this.objectsValidator = new ObjectsValidator(this.TestParameters);
			this.choicesValidator = new ChoicesValidator(this.TestParameters);
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, createdObjectArtifactTypeId).ConfigureAwait(false);
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			ImportHelper.ImportDefaultTestData(this.TestParameters);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
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
			Settings settings = NativeImportSettingsProvider.NativeControlNumberIdentifierObjectImportSettings(artifactTypeId);

			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

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
			this.objectsValidator.ValidateImportedObjectsCountAndNotEmptyFieldsValues(
					NumberOfFilesToImport,
					false,
					new[] { WellKnownFields.ControlNumber },
					artifactTypeId);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.NotInCompatibility)]
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db0852938789")]
		public void ShouldImportFolders()
		{
			// ARRANGE
			ForceClient(TapiClient.Direct);

			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
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
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("3723e0e9-2ce1-472b-b655-8fbffb515c1a")]
		public void ShouldAppendOverlayDocumentsAndMoveToNewFolders()
		{
			// ARRANGE
			ForceClient(TapiClient.Direct);

			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
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
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("700bda86-6e9a-43c1-a69c-2a1972cba4f8")]
		public async Task ShouldImportDocumentWithChoices(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.NativeControlNumberIdentifierObjectImportSettings(artifactTypeId);

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
			IEnumerable<string> confidentialDesignation = RandomPathGenerator.GetChoiceGenerator(
				numOfDifferentElements: 100,
				maxElementLength: 200,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter).ToEnumerable().Take(numberOfDocumentsToImport).ToList();

			// Overlay replace.
			IEnumerable<string> privilegeDesignation = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter).ToEnumerable(nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter).Take(numberOfDocumentsToImport).ToList();

			// Overlay append.
			IEnumerable<string> classificationIndex = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter).ToEnumerable(nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter).Take(numberOfDocumentsToImport).ToList();

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(WellKnownFields.ConfidentialDesignation, confidentialDesignation)
				.AddField(WellKnownFields.PrivilegeDesignation, privilegeDesignation).AddField(
					WellKnownFields.ClassificationIndex,
					classificationIndex).Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));
			this.objectsValidator.ValidateImportedObjectsCountAndNotEmptyFieldsValues(numberOfDocumentsToImport, true, new[] { WellKnownFields.ControlNumber, WellKnownFields.PrivilegeDesignation, WellKnownFields.ConfidentialDesignation, WellKnownFields.ClassificationIndex }, artifactTypeId);
			Dictionary<string, IEnumerable<string>> fieldsAndValuesToValidate = new Dictionary<string, IEnumerable<string>>();
			fieldsAndValuesToValidate.Add(WellKnownFields.ConfidentialDesignation, confidentialDesignation);
			fieldsAndValuesToValidate.Add(WellKnownFields.PrivilegeDesignation, privilegeDesignation);
			fieldsAndValuesToValidate.Add(WellKnownFields.ClassificationIndex, classificationIndex);

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
		[IdentifiedTest("e555aa7f-9976-4a74-87b4-577853209b57")]
		public void ShouldImportDocumentWithChoices2(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.NativeControlNumberIdentifierObjectImportSettings(artifactTypeId);
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
			this.objectsValidator.ValidateImportedObjectsCountAndNotEmptyFieldsValues(
				importData.Length,
				true,
				new[] { WellKnownFields.ControlNumber, WellKnownFields.PrivilegeDesignation, WellKnownFields.ConfidentialDesignation },
				artifactTypeId);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("13dc1d17-4a2b-4b48-9015-b61e58bc5168")]
		public async Task ShouldImportDocumentWithObjects(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.NativeControlNumberIdentifierObjectImportSettings(artifactTypeId);

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

			IEnumerable<string> originatingImagingDocumentError = RandomPathGenerator.GetObjectGenerator(
				numOfDifferentElements: 100,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter).ToEnumerable().Take(numberOfDocumentsToImport).ToList();

			IEnumerable<string> domainsEmailTo = RandomPathGenerator.GetObjectGenerator(
					numOfDifferentElements: 300,
					maxElementLength: 255,
					numOfDifferentPaths: 100,
					maxPathDepth: 1,
					multiValueDelimiter: multiValueDelimiter).ToEnumerable()
				.RandomUniqueBatch(2, multiValueDelimiter)
				.Take(numberOfDocumentsToImport).ToList();

			IEnumerable<string> domainsEmailFrom = RandomPathGenerator.GetObjectGenerator(
					numOfDifferentElements: 400,
					maxElementLength: 255,
					numOfDifferentPaths: 100,
					maxPathDepth: 1,
					multiValueDelimiter: multiValueDelimiter).ToEnumerable()
				.RandomUniqueBatch(5, multiValueDelimiter)
				.Take(numberOfDocumentsToImport).ToList();

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(WellKnownFields.OriginatingImagingDocumentError, originatingImagingDocumentError)
				.AddField(WellKnownFields.DomainsEmailTo, domainsEmailTo).AddField(
					WellKnownFields.DomainsEmailFrom,
					domainsEmailFrom).Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));

			Dictionary<string, IEnumerable<string>> fieldsAndValuesToValidate = new Dictionary<string, IEnumerable<string>>();
			fieldsAndValuesToValidate.Add(WellKnownFields.OriginatingImagingDocumentError, originatingImagingDocumentError);
			fieldsAndValuesToValidate.Add(WellKnownFields.DomainsEmailTo, domainsEmailTo);
			fieldsAndValuesToValidate.Add(WellKnownFields.DomainsEmailFrom, domainsEmailFrom);

			await this.objectsValidator.ValidateObjectFieldsValuesWithExpected(
				controlNumber,
				fieldsAndValuesToValidate,
				multiValueDelimiter,
				artifactTypeId).ConfigureAwait(false);
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.ImportObject)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("bfd19759-3ef5-4752-84ab-268e9fb54e3d")]
		public async Task ShouldImportDocumentWithObjects2(
			[Values(ArtifactType.Document, ArtifactType.ObjectType)] ArtifactType artifactType,
			[Values(OverwriteModeEnum.Append)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			int artifactTypeId = GetArtifactTypeIdForTest(artifactType);
			Settings settings = NativeImportSettingsProvider.NativeControlNumberIdentifierObjectImportSettings(artifactTypeId);

			// Prepare data for import under test
			settings.OverwriteMode = overwriteMode;
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			char multiValueDelimiter = settings.MultiValueDelimiter;

			int numberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 2;
			int numberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
			int numberOfDocumentsToImport = numberOfDocumentsToAppend + numberOfDocumentsToOverlay;

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(overwriteMode, numberOfDocumentsToAppend, $"{nameof(this.ShouldImportDocumentWithObjects)}{overwriteMode}").ToList();

			IEnumerable<string> domainsEmailTo = new List<string> { "UÁØ]7$(ÝÄ2H", " uáØ]7$(ÝÄ2H", "hÜ)ßêÏuNëiìÚÃOÒËetYÏÝÛþCLS4MÉI2dwÔ8má] {aLÑ<1dûxcêx3)ýëÎÝçrvzIÆÂWgCï5Dé'tÝNp*â0èX|éßGÿ[yèjÑúëi!8Ç.åÊA,b@ û~LÐZÄûÖ" };

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(WellKnownFields.DomainsEmailTo, domainsEmailTo).Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));

			Dictionary<string, IEnumerable<string>> fieldsAndValuesToValidate = new Dictionary<string, IEnumerable<string>>();
			fieldsAndValuesToValidate.Add(WellKnownFields.DomainsEmailTo, domainsEmailTo);

			await this.objectsValidator.ValidateObjectFieldsValuesWithExpected(
				controlNumber,
				fieldsAndValuesToValidate,
				multiValueDelimiter,
				artifactTypeId).ConfigureAwait(false);
		}

		private static IEnumerable<string> GetControlNumberEnumerable(
			OverwriteModeEnum overwriteMode,
			int numberOfDocumentsToAppend,
			string appendToName)
		{
			IEnumerable<string> controlNumber;
			if (overwriteMode == OverwriteModeEnum.Overlay || overwriteMode == OverwriteModeEnum.AppendOverlay)
			{
				controlNumber = TestData.SampleDocFiles.Select(Path.GetFileName);
			}
			else
			{
				controlNumber = Enumerable.Empty<string>();
			}

			if (overwriteMode == OverwriteModeEnum.Append || overwriteMode == OverwriteModeEnum.AppendOverlay)
			{
				controlNumber = controlNumber.Concat(
					Enumerable.Range(1, numberOfDocumentsToAppend).Select(p => $"{p}-{appendToName}"));
			}

			return controlNumber;
		}

		private int GetArtifactTypeIdForTest(ArtifactType artifactType)
		{
			int artifactTypeId = artifactType == ArtifactType.Document
				                     ? (int)ArtifactTypeID.Document
				                     : this.createdObjectArtifactTypeId;
			return artifactTypeId;
		}

		private async Task<int> CreateObjectInWorkspaceAsync()
		{
			string objectName = Guid.NewGuid().ToString();

			var objectId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, objectName).ConfigureAwait(false);
			await FieldHelper.CreateFileFieldAsync(this.TestParameters, "FilePath", objectId).ConfigureAwait(false);
			await FieldHelper.CreateSingleChoiceFieldAsync(
				this.TestParameters,
				WellKnownFields.ConfidentialDesignation,
				objectId,
				false).ConfigureAwait(false);
			await FieldHelper.CreateMultiChoiceFieldAsync(
				this.TestParameters,
				WellKnownFields.PrivilegeDesignation,
				objectId,
				false).ConfigureAwait(false);
			await FieldHelper.CreateMultiChoiceFieldAsync(
				this.TestParameters,
				WellKnownFields.ClassificationIndex,
				objectId,
				false).ConfigureAwait(false);

			var controlNumberFieldRequest = new FixedLengthFieldRequest()
				                                {
					                                Name = WellKnownFields.ControlNumber,
					                                ObjectType = new ObjectTypeIdentifier() { Name = objectName },
					                                Length = 255,
					                                IsRequired = true,
					                                IncludeInTextIndex = true,
					                                FilterType = FilterType.TextBox,
					                                AllowSortTally = true,
					                                AllowGroupBy = false,
					                                AllowPivot = false,
					                                HasUnicode = true,
					                                OpenToAssociations = false,
					                                IsRelational = false,
					                                AllowHtml = false,
					                                IsLinked = true,
					                                Wrapping = true,
				                                };
			using (IFieldManager fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(this.TestParameters))
			{
				await fieldManager.UpdateFixedLengthFieldAsync(
					this.TestParameters.WorkspaceId,
					FieldHelper.QueryIdentifierFieldId(this.TestParameters, objectName),
					controlNumberFieldRequest).ConfigureAwait(false);
			}

			int domainObjectId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, "DomainForObject")
				                     .ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(
				this.TestParameters,
				WellKnownFields.DomainsEmailFrom,
				domainObjectId,
				objectId).ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(
				this.TestParameters,
				WellKnownFields.DomainsEmailTo,
				domainObjectId,
				objectId).ConfigureAwait(false);

			int imagingObjectError = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, "Imaging Object Error")
				                         .ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(
				this.TestParameters,
				WellKnownFields.OriginatingImagingDocumentError,
				imagingObjectError,
				objectId).ConfigureAwait(false);

			return objectId;
		}
	}
}