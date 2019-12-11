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
			Assert.That(results.JobMessages, Has.Count.Positive);
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(NumberOfFilesToImport));
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

			const int NumberOfDocumentsToImport = 2000;
			var randomFolderGenerator = RandomPathGenerator.GetFolderGenerator(
				maxDepth: 100,
				numOfDifferentFolders: 25,
				numOfDifferentPaths: 100,
				maxFolderLength: 255);

			IEnumerable<FolderImportDto> importData = randomFolderGenerator
				.ToFolders(NumberOfDocumentsToImport)
				.Select((p, i) => new FolderImportDto(i.ToString(), p));

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(results.JobMessages, Has.Count.Positive);
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
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
				TestData.SampleDocFiles.Select(p => new FolderImportDto(System.IO.Path.GetFileName(p), @"\aaa \cc"));

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocumentsToImport);
			Assert.That(results, Has.Count.Positive);
			Assert.That(results, Has.Count.EqualTo(numberOfDocumentsToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("700bda86-6e9a-43c1-a69c-2a1972cba4f8")]
		public void ShouldImportDocumentWithChoices()
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();

			const int NumberOfDocumentsToImport = 2000;
			char multiValueDelimiter = settings.MultiValueDelimiter;
			char nestedValueDelimiter = settings.NestedValueDelimiter;

			this.InitializeImportApiWithUserAndPassword(settings);

			RandomPathGenerator confidentialDesignation = RandomPathGenerator.GetChoiceGenerator(
				maxDepth: 1,
				numOfDifferentFolders: 100,
				numOfDifferentPaths: 100,
				maxFolderLength: 200,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			RandomPathGenerator privilegeDesignation = RandomPathGenerator.GetChoiceGenerator(
				maxDepth: 4,
				numOfDifferentFolders: 250,
				numOfDifferentPaths: 100,
				maxFolderLength: 200,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			ImportTestJobResult results = null;
			using (var dataReader = new ZipDataReader())
			{
				dataReader.Add(WellKnownFields.ControlNumber, Enumerable.Range((2 * NumberOfDocumentsToImport) + 1, NumberOfDocumentsToImport).Select(p => p.ToString()));
				dataReader.Add(WellKnownFields.ConfidentialDesignation, confidentialDesignation.ToEnumerable(NumberOfDocumentsToImport));
				dataReader.Add(WellKnownFields.PrivilegeDesignation, privilegeDesignation.ToEnumerable(int.MaxValue, nestedValueDelimiter).RandomUniqueBatch(4, multiValueDelimiter));

				// ACT
				results = this.Execute(dataReader);
			}

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(results.JobMessages, Has.Count.Positive);
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
		}

		[Test]
		public void ShouldImportDocumentWithChoices2()
		{
			// ARRANGE
			this.InitializeImportApiWithUserAndPassword(NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings());

			DocumentWithChoicesImportDto[] importData =
			{
				new DocumentWithChoicesImportDto("100001", "qqqq", "www;eee"),
				new DocumentWithChoicesImportDto("100002", "qqqq", @"www;eee\rrr"),
			};

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(importData.Length);
			Assert.That(results.JobMessages, Has.Count.Positive);
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(importData.Length));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("13dc1d17-4a2b-4b48-9015-b61e58bc5168")]
		public void ShouldImportDocumentWithObjects()
		{
			// ARRANGE
			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();

			const int NumberOfDocumentsToImport = 2000;
			char multiValueDelimiter = settings.MultiValueDelimiter;
			this.InitializeImportApiWithUserAndPassword(settings);

			var originatingImagingDocumentError = RandomPathGenerator.GetObjectGenerator(
				maxDepth: 1,
				numOfDifferentFolders: 100,
				numOfDifferentPaths: 100,
				maxFolderLength: 255,
				multiValueDelimiter: multiValueDelimiter);

			var domainsEmailTo = RandomPathGenerator.GetObjectGenerator(
				maxDepth: 1,
				numOfDifferentFolders: 300,
				numOfDifferentPaths: 100,
				maxFolderLength: 255,
				multiValueDelimiter: multiValueDelimiter);

			var domainsEmailFrom = RandomPathGenerator.GetObjectGenerator(
				maxDepth: 1,
				numOfDifferentFolders: 400,
				numOfDifferentPaths: 100,
				maxFolderLength: 255,
				multiValueDelimiter: multiValueDelimiter);

			IEnumerable<DocumentWithObjectsImportDto> importData = Enumerable
				.Range((3 * NumberOfDocumentsToImport) + 1, NumberOfDocumentsToImport)
				.Select(p => p.ToString())
				.Zip(
					originatingImagingDocumentError.ToEnumerable(NumberOfDocumentsToImport),
					domainsEmailTo.ToEnumerable(int.MaxValue).RandomUniqueBatch(2, multiValueDelimiter),
					domainsEmailFrom.ToEnumerable(int.MaxValue).RandomUniqueBatch(5, multiValueDelimiter),
					(controlNumber, imagingDocumentError, emailTo, emailFrom) =>
						new DocumentWithObjectsImportDto(controlNumber, imagingDocumentError, emailTo, emailFrom));

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(results.JobMessages, Has.Count.Positive);
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
		}
	}
}