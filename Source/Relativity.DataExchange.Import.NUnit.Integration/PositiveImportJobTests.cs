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

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImportJobTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("f808845d-c8c9-454b-9d84-51d84be70bd1")]
		[Test]
		[Pairwise]
		public void ShouldImportTheFiles(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(true, false)] bool disableNativeLocationValidation,
			[Values(true, false)] bool disableNativeValidation)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			ForceClient(client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, NativeImportSettingsProvider.NativeFilePathSourceDocumentImportSettings);

			const int NumberOfFilesToImport = 5;
			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfFilesToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfFilesToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
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
			IEnumerable<FolderImportDto> importData = randomFolderGenerator
				.ToFolders(NumberOfDocumentsToImport)
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
		[Category(TestCategories.Integration)]
		[IdentifiedTest("700bda86-6e9a-43c1-a69c-2a1972cba4f8")]
		public void ShouldImportDocumentWithChoices(
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
			settings.OverwriteMode = overwriteMode;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			char multiValueDelimiter = settings.MultiValueDelimiter;
			char nestedValueDelimiter = settings.NestedValueDelimiter;

			int numberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 201;
			int numberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
			int numberOfDocumentsToImport = numberOfDocumentsToAppend + numberOfDocumentsToOverlay;

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(overwriteMode, numberOfDocumentsToAppend, $"{nameof(this.ShouldImportDocumentWithChoices)}{overwriteMode}");

			IEnumerable<string> confidentialDesignation = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 100,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 1,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter)
				.ToEnumerable();

			// Overlay replace.
			IEnumerable<string> privilegeDesignation = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter)
				.ToEnumerable(nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter);

			// Overlay append.
			IEnumerable<string> classificationIndex = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter)
				.ToEnumerable(nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(WellKnownFields.ConfidentialDesignation, confidentialDesignation)
				.AddField(WellKnownFields.PrivilegeDesignation, privilegeDesignation)
				.AddField(WellKnownFields.ClassificationIndex, classificationIndex)
				.Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("e555aa7f-9976-4a74-87b4-577853209b57")]
		public void ShouldImportDocumentWithChoices2()
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			DocumentWithChoicesImportDto[] importData =
			{
				new DocumentWithChoicesImportDto("100009", "qqqq2", "www;eee"),
				new DocumentWithChoicesImportDto("100010", "qqqq2", @"www;eee\rrr"),
			};

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, importData.Length);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(importData.Length));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("13dc1d17-4a2b-4b48-9015-b61e58bc5168")]
		public void ShouldImportDocumentWithObjects(
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
			settings.OverwriteMode = overwriteMode;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			char multiValueDelimiter = settings.MultiValueDelimiter;

			int numberOfDocumentsToAppend = overwriteMode == OverwriteModeEnum.Overlay ? 0 : 2010;
			int numberOfDocumentsToOverlay = overwriteMode == OverwriteModeEnum.Append ? 0 : TestData.SampleDocFiles.Count();
			int numberOfDocumentsToImport = numberOfDocumentsToAppend + numberOfDocumentsToOverlay;

			IEnumerable<string> controlNumber = GetControlNumberEnumerable(overwriteMode, numberOfDocumentsToAppend, $"{nameof(this.ShouldImportDocumentWithObjects)}{overwriteMode}");

			IEnumerable<string> originatingImagingDocumentError = RandomPathGenerator.GetObjectGenerator(
				numOfDifferentElements: 100,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter)
			.ToEnumerable();

			IEnumerable<string> domainsEmailTo = RandomPathGenerator.GetObjectGenerator(
				numOfDifferentElements: 300,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter)
			.ToEnumerable()
			.RandomUniqueBatch(2, multiValueDelimiter);

			IEnumerable<string> domainsEmailFrom = RandomPathGenerator.GetObjectGenerator(
				numOfDifferentElements: 400,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter)
			.ToEnumerable()
			.RandomUniqueBatch(5, multiValueDelimiter);

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumber)
				.AddField(WellKnownFields.OriginatingImagingDocumentError, originatingImagingDocumentError)
				.AddField(WellKnownFields.DomainsEmailTo, domainsEmailTo)
				.AddField(WellKnownFields.DomainsEmailFrom, domainsEmailFrom)
				.Build();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));
		}

		private static IEnumerable<string> GetControlNumberEnumerable(OverwriteModeEnum overwriteMode, int numberOfDocumentsToAppend, string appendToName)
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
					Enumerable.Range(1, numberOfDocumentsToAppend)
						.Select(p => $"{p}-{appendToName}"));
			}

			return controlNumber;
		}
	}
}