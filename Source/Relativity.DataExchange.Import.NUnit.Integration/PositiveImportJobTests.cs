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
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImportJobTests : ImportJobTestBase<ImportBulkArtifactJob, Settings>
	{
		public PositiveImportJobTests()
			: base(new NativeImportApiSetUp())
		{
		}

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

			this.InitializeImportApiWithUserAndPassword(NativeImportSettingsProvider.GetNativeFilePathSourceDocumentImportSettings());

			const int NumberOfFilesToImport = 5;
			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport);

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfFilesToImport);
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

			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			this.InitializeImportApiWithUserAndPassword(settings);

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
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
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

			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			settings.MoveDocumentsInAppendOverlayMode = true;
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			this.InitializeImportApiWithUserAndPassword(settings);

			int numberOfDocumentsToImport = TestData.SampleDocFiles.Count();
			IEnumerable<FolderImportDto> importData =
				TestData.SampleDocFiles.Select(p => new FolderImportDto(Path.GetFileName(p), @"\aaa \cc"));

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("700bda86-6e9a-43c1-a69c-2a1972cba4f8")]
		public void ShouldImportDocumentWithChoices(
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();
			settings.OverwriteMode = overwriteMode;

			this.InitializeImportApiWithUserAndPassword(settings);

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
				.ToEnumerable(int.MaxValue);

			// Overlay replace.
			IEnumerable<string> privilegeDesignation = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter)
				.ToEnumerable(int.MaxValue, nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter);

			// Overlay append.
			IEnumerable<string> classificationIndex = RandomPathGenerator.GetChoiceGenerator(
					numOfDifferentElements: 250,
					maxElementLength: 200,
					numOfDifferentPaths: 100,
					maxPathDepth: 4,
					multiValueDelimiter: multiValueDelimiter,
					nestedValueDelimiter: nestedValueDelimiter)
				.ToEnumerable(int.MaxValue, nestedValueDelimiter)
				.RandomUniqueBatch(4, multiValueDelimiter);

			ImportTestJobResult results;
			using (var dataReader = new ZipDataReader())
			{
				dataReader.Add(WellKnownFields.ControlNumber, controlNumber);
				dataReader.Add(WellKnownFields.ConfidentialDesignation, confidentialDesignation);
				dataReader.Add(WellKnownFields.PrivilegeDesignation, privilegeDesignation);
				dataReader.Add(WellKnownFields.ClassificationIndex, classificationIndex);

				// ACT
				results = this.Execute(dataReader);
			}

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocumentsToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("e555aa7f-9976-4a74-87b4-577853209b57")]
		public void ShouldImportDocumentWithChoices2()
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;

			this.InitializeImportApiWithUserAndPassword(settings);

			DocumentWithChoicesImportDto[] importData =
			{
				new DocumentWithChoicesImportDto("100009", "qqqq2", "www;eee"),
				new DocumentWithChoicesImportDto("100010", "qqqq2", @"www;eee\rrr"),
			};

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(importData.Length);
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
			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();
			settings.OverwriteMode = overwriteMode;

			this.InitializeImportApiWithUserAndPassword(settings);

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
			.ToEnumerable(int.MaxValue);

			IEnumerable<string> domainsEmailTo = RandomPathGenerator.GetObjectGenerator(
				numOfDifferentElements: 300,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter)
			.ToEnumerable(int.MaxValue)
			.RandomUniqueBatch(2, multiValueDelimiter);

			IEnumerable<string> domainsEmailFrom = RandomPathGenerator.GetObjectGenerator(
				numOfDifferentElements: 400,
				maxElementLength: 255,
				numOfDifferentPaths: 100,
				maxPathDepth: 1,
				multiValueDelimiter: multiValueDelimiter)
			.ToEnumerable(int.MaxValue)
			.RandomUniqueBatch(5, multiValueDelimiter);

			ImportTestJobResult results;
			using (var dataReader = new ZipDataReader())
			{
				dataReader.Add(WellKnownFields.ControlNumber, controlNumber);
				dataReader.Add(WellKnownFields.OriginatingImagingDocumentError, originatingImagingDocumentError);
				dataReader.Add(WellKnownFields.DomainsEmailTo, domainsEmailTo);
				dataReader.Add(WellKnownFields.DomainsEmailFrom, domainsEmailFrom);

				// ACT
				results = this.Execute(dataReader);
			}

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocumentsToImport);
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