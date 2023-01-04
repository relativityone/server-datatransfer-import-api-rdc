// <copyright file="KeplerSearchManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Service;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Tested class is complex.")]
	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.RelativityDesktopClient.Export]
	public class KeplerSearchManagerTests : KeplerServiceTestBase
	{
		private readonly int[] nonExistingArtifactIds = { 4643643, 431421421, 41454253 };

		private readonly List<string> searchNames = new List<string>();
		private Dictionary<string, int> controlNumberToArtifactIdMapping;
		private string firstDocumentControlNumber;
		private string lastDocumentControlNumber;
		private List<string> originalNativeFileNames;
		private int rdoWithFilesObjectTypeArtifactId;
		private int fileFieldArtifactId;

		private int searchWithFirstDocumentId;
		private int notProducedProductionId;
		private int productionWithFirstDocumentId;
		private int productionWithLastDocumentId;

		public KeplerSearchManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task OneTimeSetUpAsync()
		{
			this.ImportTestData();
			var searchWithLastDocument = await this.CreateSavedSearchesAsync().ConfigureAwait(false);
			await this.CreateProductionsAsync(searchWithLastDocument).ConfigureAwait(false);
			await this.CreateRdoWithFileFieldAsync().ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
			await ProductionHelper.DeleteAllProductionsAsync(this.TestParameters).ConfigureAwait(false);
		}

		[IdentifiedTest("594e397f-abfe-437e-9bbe-c3e76525eade")]
		public void ShouldRetrieveNativesForSearch()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForSearch(this.TestParameters.WorkspaceId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				var actualListOfNatives = ConvertDataSetToListOfFiles(actualResult).ToList();

				Assert.That(actualListOfNatives.Select(x => x.documentArtifactId), Is.EquivalentTo(documentsArtifactIds), "It should return data for all documents.");
				Assert.That(actualListOfNatives.Select(x => x.originalFileName), Is.EquivalentTo(this.originalNativeFileNames), "It should return correct original file names.");
				Assert.That(actualListOfNatives.Select(x => x.filePath), Has.All.Not.Null.And.Not.Empty, "File path should not be empty");
			}
		}

		[IdentifiedTest("c1ae7a8d-b966-44ea-9f76-f3c96c984065")]
		public void ShouldThrowExceptionWhenRetrieveNativesForSearchForNonExistingWorkspace()
		{
			// arrange
			var documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToList();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveNativesForSearch(NonExistingWorkspaceId, commaSeparatedDocumentArtifactIds),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("8436664b-b9de-4e8e-b24b-86269254b6cb")]
		public void ShouldReturnEmptyDataSetWhenRetrieveNativesForSearchForNonExistingDocuments()
		{
			// arrange
			string commaSeparatedDocumentArtifactIds = string.Join(",", this.nonExistingArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForSearch(this.TestParameters.WorkspaceId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not exist.");
			}
		}

		[IdentifiedTest("04b61be7-5965-4f15-a461-e38695e2061c")]
		public async Task ShouldReturnEmptyDataSetWhenRetrieveNativesForSearchForDocumentsWithoutNativesAsync()
		{
			// arrange
			string[] controlNumbersWithoutNatives =
				{
					"DOC1_NO_NATIVE",
					"DOC2_NO_NATIVE",
				};
			var importDataSource = ImportDataSourceBuilder
				.New()
				.AddField(WellKnownFields.ControlNumber, controlNumbersWithoutNatives)
				.Build();
			ImportHelper.ImportDocumentsMetadata(this.TestParameters, importDataSource);

			this.LoadDocumentsFromWorkspace();
			Assert.That(
				this.controlNumberToArtifactIdMapping,
				Has.Count.EqualTo(this.originalNativeFileNames.Count + controlNumbersWithoutNatives.Length),
				"Arrange failed. Number of imported documents should be equal to number of test files");

			List<int> documentWithoutNativesArtifactIds = this.controlNumberToArtifactIdMapping
				.Where(x => controlNumbersWithoutNatives.Contains(x.Key))
				.Select(x => x.Value)
				.ToList();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentWithoutNativesArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForSearch(this.TestParameters.WorkspaceId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not have native files.");
			}

			foreach (var artifactId in documentWithoutNativesArtifactIds)
			{
				await RdoHelper.DeleteObjectAsync(this.TestParameters, artifactId).ConfigureAwait(false);
			}
		}

		[IgnoreIfVersionLowerThan(RelativityVersion.MayappleExportPDFs)] // Method RetrievePdfForSearch was added in this version
		[IdentifiedTest("1833063a-aac9-4dca-8d6e-599c0a9e166f")]
		public void ShouldReturnEmptyDataSetWhenRetrievePdfForSearchForDocumentsWithoutPdfs()
		{
			// arrange
			var documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToList();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrievePdfForSearch(this.TestParameters.WorkspaceId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not have PDF files.");
			}
		}

		[IgnoreIfVersionLowerThan(RelativityVersion.MayappleExportPDFs)] // Method RetrievePdfForSearch was added in this version
		[IdentifiedTest("05628321-3d7e-4d8e-99eb-37eca243bc3e")]
		public void ShouldReturnEmptyDataSetWhenRetrievePdfForSearchForNonExistingDocuments()
		{
			// arrange
			string commaSeparatedDocumentArtifactIds = string.Join(",", this.nonExistingArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrievePdfForSearch(this.TestParameters.WorkspaceId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not exist.");
			}
		}

		[IgnoreIfVersionLowerThan(RelativityVersion.MayappleExportPDFs)] // Method RetrievePdfForSearch was added in this version
		[IdentifiedTest("dbbc2bb5-6ed9-4057-8196-231118caee01")]
		public void ShouldThrowExceptionWhenRetrievePdfForSearchForNonExistingWorkspace()
		{
			// arrange
			var documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToList();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrievePdfForSearch(NonExistingWorkspaceId, commaSeparatedDocumentArtifactIds),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("e528441e-36f4-4e47-b217-e3d46e2e9028")]
		public async Task ShouldRetrieveFilesForDynamicObjectsAsync()
		{
			// arrange
			string[] objectsNames =
				{
					"RDO1_WITH_NATIVE",
					"RDO2_WITH_NATIVE",
				};
			string[] filePaths = TestData.SampleDocFiles.Take(2).ToArray();

			var dataSource = ImportDataSourceBuilder
				.New()
				.AddField(WellKnownFields.RdoIdentifier, objectsNames)
				.AddField(WellKnownFields.FilePath, filePaths)
				.Build();
			ImportHelper.ImportObjectsMetadataWithFiles(this.TestParameters, this.rdoWithFilesObjectTypeArtifactId, dataSource);

			int[] objectsArtifactIds = RdoHelper.QueryRelativityObjects(
					TestParameters,
					this.rdoWithFilesObjectTypeArtifactId,
					new string[0])
				.Select(x => x.ArtifactID)
				.ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveFilesForDynamicObjects(this.TestParameters.WorkspaceId, this.fileFieldArtifactId, objectsArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(5), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Has.Count.EqualTo(objectsNames.Length), "It should return files for all objects.");
			}

			await RdoHelper.DeleteAllObjectsByTypeAsync(TestParameters, this.rdoWithFilesObjectTypeArtifactId).ConfigureAwait(false);
		}

		[IdentifiedTest("3641e41b-5226-4de3-9d33-4518778186a8")]
		public void ShouldReturnEmptyDataSetWhenRetrieveFilesForDynamicObjectsForNonExistingObjects()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveFilesForDynamicObjects(this.TestParameters.WorkspaceId, this.fileFieldArtifactId, this.nonExistingArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(5), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when objects does not exist.");
			}
		}

		[IdentifiedTest("a68d4e1b-b2c0-4834-9dd4-76d3483c1bab")]
		public async Task ShouldReturnEmptyDataSetWhenRetrieveFilesForDynamicObjectsForObjectsWithoutFilesAsync()
		{
			// arrange
			int objectArtifactId = RdoHelper.CreateObjectTypeInstance(
				this.TestParameters,
				this.rdoWithFilesObjectTypeArtifactId,
				new Dictionary<string, object> { [WellKnownFields.RdoIdentifier] = "RDO1_WITHOUT_FILE" });

			int[] objectsArtifactIds = { objectArtifactId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveFilesForDynamicObjects(this.TestParameters.WorkspaceId, this.fileFieldArtifactId, objectsArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(5), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when objects does not exist.");
			}

			await RdoHelper.DeleteAllObjectsByTypeAsync(TestParameters, this.rdoWithFilesObjectTypeArtifactId).ConfigureAwait(false);
		}

		[IdentifiedTest("f2062e4c-2243-4bb1-96a1-92bd386ba378")]
		public void ShouldRetrieveImagesForSearch()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesForSearch(this.TestParameters.WorkspaceId, documentsArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(15), "Result should have 15 columns");
				var actualFiles = new List<(int documentArtifactId, string originalFileName, string filePath)>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					int documentArtifactId = dataRow.Field<int>(2);
					string originalFileName = dataRow.Field<string>(3);
					string filePath = dataRow.Field<string>(8);

					actualFiles.Add((documentArtifactId, originalFileName, filePath));
				}

				Assert.That(actualFiles.Select(x => x.documentArtifactId).Distinct(), Is.EquivalentTo(documentsArtifactIds), "It should return data for all documents.");
				Assert.That(actualFiles.Select(x => x.documentArtifactId).Count(), Is.EqualTo(4 * documentsArtifactIds.Length), "It should 4 images for each document.");
				Assert.That(actualFiles.Select(x => x.originalFileName), Has.All.Not.Null.And.Not.Empty, "Original file name should not be empty.");
				Assert.That(actualFiles.Select(x => x.filePath), Has.All.Not.Null.And.Not.Empty, "File path should not be empty");
			}
		}

		[IdentifiedTest("fa1faa10-d239-44dd-a1f4-0fb079220cd5")]
		public void ShouldThrowExceptionWhenRetrieveImagesForSearchForNonExistingWorkspace()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImagesForSearch(NonExistingWorkspaceId, documentsArtifactIds),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("60896e63-2ab1-4373-8a1c-67eb4ae36e86")]
		public void ShouldReturnEmptyDataSetWhenRetrieveImagesForSearchForNonExistingDocuments()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesForSearch(this.TestParameters.WorkspaceId, this.nonExistingArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(15), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not exist.");
			}
		}

		[IdentifiedTest("1728832e-8229-40b1-9954-20fe792b226c")]
		public async Task ShouldReturnEmptyDataSetWhenRetrieveImagesForSearchForDocumentsWithoutImagesAsync()
		{
			// arrange
			string[] controlNumbersWithoutImages =
				{
					"DOC1_NO_NATIVE",
					"DOC2_NO_NATIVE",
				};
			var importDataSource = ImportDataSourceBuilder
				.New()
				.AddField(WellKnownFields.ControlNumber, controlNumbersWithoutImages)
				.Build();
			ImportHelper.ImportDocumentsMetadata(this.TestParameters, importDataSource);

			this.LoadDocumentsFromWorkspace();
			Assert.That(
				this.controlNumberToArtifactIdMapping,
				Has.Count.EqualTo(this.originalNativeFileNames.Count + controlNumbersWithoutImages.Length),
				"Arrange failed. Number of imported documents should be equal to number of test files");

			int[] documentWithoutImagesArtifactIds = this.controlNumberToArtifactIdMapping
				.Where(x => controlNumbersWithoutImages.Contains(x.Key))
				.Select(x => x.Value)
				.ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesForSearch(this.TestParameters.WorkspaceId, documentWithoutImagesArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(15), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not have native files.");
			}

			foreach (var artifactId in documentWithoutImagesArtifactIds)
			{
				await RdoHelper.DeleteObjectAsync(this.TestParameters, artifactId).ConfigureAwait(false);
			}
		}

		[IdentifiedTest("d8a031c9-a1ef-4816-91ed-5ad57414c998")]
		public void ShouldRetrieveNativesForProduction()
		{
			// arrange
			var documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToList();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForProduction(this.TestParameters.WorkspaceId, this.productionWithFirstDocumentId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				var actualFiles = new List<(int documentArtifactId, string originalFileName, string filePath)>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					int documentArtifactId = dataRow.Field<int>(2);
					string originalFileName = dataRow.Field<string>(3);
					string filePath = dataRow.Field<string>(8);

					actualFiles.Add((documentArtifactId, originalFileName, filePath));
				}

				int expectedDocumentIdentifier = this.controlNumberToArtifactIdMapping[this.firstDocumentControlNumber];
				Assert.That(actualFiles.Count(), Is.EqualTo(1), "It should return single native.");
				Assert.That(actualFiles.Select(x => x.documentArtifactId).Single(), Is.EqualTo(expectedDocumentIdentifier), "It should return data for production document.");
				Assert.That(actualFiles.Select(x => x.originalFileName), Has.All.Not.Null.And.Not.Empty, "Original file names should not be empty.");
				Assert.That(actualFiles.Select(x => x.filePath), Has.All.Not.Null.And.Not.Empty, "File path should not be empty.");
			}
		}

		[IdentifiedTest("f47140a4-31da-4684-bb2f-72ade42fecbd")]
		public void ShouldReturnEmptyDataSetWhenRetrieveNativesForProductionForDocumentsNotInProduction()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping
				.Where(x => x.Key != this.firstDocumentControlNumber)
				.Select(x => x.Value)
				.ToArray();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForProduction(this.TestParameters.WorkspaceId, this.notProducedProductionId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty data set when documents are not in production.");
			}
		}

		[IdentifiedTest("4c8f7e98-a405-49ff-92cd-8388dbaa33d2")]
		public void ShouldReturnEmptyDataSetWhenRetrieveNativesForProductionForNotProducedProduction()
		{
			// arrange
			var documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToList();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForProduction(this.TestParameters.WorkspaceId, this.notProducedProductionId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty data set when production was not produced.");
			}
		}

		[IdentifiedTest("f4882392-9736-4ca2-83cf-3a03300a2064")]
		public void ShouldThrowExceptionWhenRetrieveNativesForProductionForNonExistingWorkspace()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveNativesForProduction(NonExistingWorkspaceId, this.productionWithFirstDocumentId, commaSeparatedDocumentArtifactIds),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("9336e682-2121-433e-8bfb-1e6637913b15")]
		public void ShouldReturnEmptyDataSetWhenRetrieveNativesForProductionForNonExistingProduction()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();
			string commaSeparatedDocumentArtifactIds = string.Join(",", documentsArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForProduction(this.TestParameters.WorkspaceId, NonExistingProductionId, commaSeparatedDocumentArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when production does not exist.");
			}
		}

		[IdentifiedTest("e09a40bd-4777-41fb-a742-0ae19fa53faf")]
		public void ShouldReturnEmptyDataSetWhenRetrieveNativesForProductionForNonExistingDocuments()
		{
			// arrange
			string nonExistingDocumentsArtifactIds = string.Join(",", this.nonExistingArtifactIds);

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveNativesForProduction(this.TestParameters.WorkspaceId, this.productionWithFirstDocumentId, nonExistingDocumentsArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(13), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not exist.");
			}
		}

		[IdentifiedTest("c45161ab-e44d-4f3e-865f-bfc6501fb65b")]
		public void ShouldRetrieveImagesForProductionDocuments()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping
				.Where(x => x.Key == this.firstDocumentControlNumber)
				.Select(x => x.Value)
				.ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesForProductionDocuments(this.TestParameters.WorkspaceId, documentsArtifactIds, this.productionWithFirstDocumentId);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(10), "Result should have 10 columns");
				var actualFiles = new List<(int documentArtifactId, string originalFileName, string filePath)>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					int documentArtifactId = dataRow.Field<int>(0);
					string originalFileName = dataRow.Field<string>(5);
					string filePath = dataRow.Field<string>(6);

					actualFiles.Add((documentArtifactId, originalFileName, filePath));
				}

				int expectedDocumentIdentifier = this.controlNumberToArtifactIdMapping[this.firstDocumentControlNumber];
				Assert.That(actualFiles.Count(), Is.EqualTo(4), "It should return images for a single document.");
				Assert.That(actualFiles.Select(x => x.documentArtifactId).Distinct().Single(), Is.EqualTo(expectedDocumentIdentifier), "It should return data for production document.");
				Assert.That(actualFiles.Select(x => x.originalFileName), Has.All.Not.Null.And.Not.Empty, "Original file names should not be empty.");
				Assert.That(actualFiles.Select(x => x.filePath), Has.All.Not.Null.And.Not.Empty, "File path should not be empty.");
			}
		}

		[IdentifiedTest("76e76603-4a8d-4e48-a545-e146538aa2c9")]
		public void ShouldReturnEmptyDataSetWhenRetrieveImagesForProductionDocumentsForDocumentsNotInProduction()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping
				.Where(x => x.Key != this.firstDocumentControlNumber)
				.Select(x => x.Value)
				.ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesForProductionDocuments(this.TestParameters.WorkspaceId, documentsArtifactIds, this.productionWithFirstDocumentId);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(10), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty data set when documents are not in production.");
			}
		}

		[IdentifiedTest("55e997c6-44a0-4952-b4e3-4053f5da24dd")]
		public void ShouldThrowExceptionWhenRetrieveImagesForProductionDocumentsForNotProducedProduction()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImagesForProductionDocuments(this.TestParameters.WorkspaceId, documentsArtifactIds, this.notProducedProductionId),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contains("Read Failed"));
			}
		}

		[IdentifiedTest("53043c94-685c-4180-89a2-3a18d4165492")]
		public void ShouldThrowExceptionWhenRetrieveImagesForProductionDocumentsForNonExistingWorkspace()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImagesForProductionDocuments(NonExistingWorkspaceId, documentsArtifactIds, this.productionWithFirstDocumentId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("1057b575-377f-451b-94c1-49141f15b738")]
		public void ShouldThrowExceptionWhenRetrieveImagesForProductionDocumentsForNonExistingProduction()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImagesForProductionDocuments(this.TestParameters.WorkspaceId, documentsArtifactIds, NonExistingProductionId),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contains("Read Failed"));
			}
		}

		[IdentifiedTest("97bbbd58-3ff6-450b-ac1c-5b4ba2dcebb0")]
		public void ShouldReturnEmptyDataSetWhenRetrieveImagesForProductionDocumentsForNonExistingDocuments()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesForProductionDocuments(this.TestParameters.WorkspaceId, this.nonExistingArtifactIds, this.productionWithFirstDocumentId);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(10), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not exist.");
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Tested method is complex.")]
		[IdentifiedTestCase("b53dc38c-6a9a-4792-8954-4c1f8dca0e22", false)]
		[IdentifiedTestCase("4198c0a3-7288-4932-9742-665b8ce1885c", true)]
		public void ShouldRetrieveImagesByProductionIDsAndDocumentIDsForExport(bool useSecondProduction)
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping
				.Select(x => x.Value)
				.ToArray();

			int[] productionPrecedence = useSecondProduction ?
				new[] { this.productionWithFirstDocumentId, this.productionWithLastDocumentId } :
				new[] { this.productionWithFirstDocumentId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesByProductionIDsAndDocumentIDsForExport(this.TestParameters.WorkspaceId, productionPrecedence, documentsArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(11), "Result should have correct number of columns");
				var actualFiles = new List<(int documentArtifactId, int productionArtifactId, string originalFileName, string filePath)>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					int documentArtifactId = dataRow.Field<int>(0);
					int productionArtifactId = dataRow.Field<int>(1);
					string originalFileName = dataRow.Field<string>(6);
					string filePath = dataRow.Field<string>(7);

					actualFiles.Add((documentArtifactId, productionArtifactId, originalFileName, filePath));
				}

				int expectedNumberOfFiles = useSecondProduction ? 8 : 4;
				Assert.That(actualFiles.Count, Is.EqualTo(expectedNumberOfFiles), "It should return correct number of images.");

				int firstDocumentId = this.controlNumberToArtifactIdMapping[this.firstDocumentControlNumber];
				var filesForFirstDocument = actualFiles.Where(x => x.documentArtifactId == firstDocumentId).ToList();
				Assert.That(filesForFirstDocument.Select(x => x.productionArtifactId), Has.All.EqualTo(this.productionWithFirstDocumentId), "It should return correct production id.");

				if (useSecondProduction)
				{
					int lastDocumentId = this.controlNumberToArtifactIdMapping[this.lastDocumentControlNumber];
					var filesForLastDocument = actualFiles.Where(x => x.documentArtifactId == lastDocumentId).ToList();
					Assert.That(filesForLastDocument.Select(x => x.productionArtifactId), Has.All.EqualTo(this.productionWithLastDocumentId), "It should return correct production id.");
				}

				Assert.That(actualFiles.Select(x => x.originalFileName), Has.All.Not.Null.And.Not.Empty, "Original file names should not be empty.");
				Assert.That(actualFiles.Select(x => x.filePath), Has.All.Not.Null.And.Not.Empty, "File path should not be empty.");
			}
		}

		[IdentifiedTestCase("90db9daa-731f-4ff6-a665-75bf3eafc404", false)]
		[IdentifiedTestCase("eb3c55c6-c6e5-4d3f-bf81-ced164e8dcbe", true)]
		public void ShouldReturnEmptyDataSetWhenRetrieveImagesByProductionIDsAndDocumentIDsForExportForDocumentsNotInProduction(bool addAdditionalProduction)
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping
				.Where(x => x.Key != this.firstDocumentControlNumber && x.Key != this.lastDocumentControlNumber)
				.Select(x => x.Value)
				.ToArray();

			int[] productionPrecedence = addAdditionalProduction ?
				new[] { this.productionWithFirstDocumentId, this.productionWithLastDocumentId } :
				new[] { this.productionWithFirstDocumentId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesByProductionIDsAndDocumentIDsForExport(this.TestParameters.WorkspaceId, productionPrecedence, documentsArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(11), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty data set when documents are not in production.");
			}
		}

		[IdentifiedTest("b211aa1f-dd2b-4e0a-84d9-c5b652a11142")]
		public void ShouldThrowExceptionWhenRetrieveImagesByProductionIDsAndDocumentIDsForExportForNotProducedProduction()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();
			int[] productionPrecedence = { this.notProducedProductionId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImagesByProductionIDsAndDocumentIDsForExport(this.TestParameters.WorkspaceId, productionPrecedence, documentsArtifactIds),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contains("Read Failed"));
			}
		}

		[IdentifiedTest("568b4e01-7738-4bd0-90f4-ff13e27515d5")]
		public void ShouldThrowExceptionWhenRetrieveImagesByProductionIDsAndDocumentIDsForExportForNonExistingWorkspace()
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();
			int[] productionPrecedence = { this.productionWithFirstDocumentId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImagesByProductionIDsAndDocumentIDsForExport(NonExistingWorkspaceId, productionPrecedence, documentsArtifactIds),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTestCase("40fe7d2b-f0d7-4a59-b40e-a71f17b87ee7", false)]
		[IdentifiedTestCase("2d2b0b37-91ed-4139-9048-895cd288f16f", true)]
		public void ShouldThrowExceptionWhenRetrieveImagesByProductionIDsAndDocumentIDsForExportForNonExistingProduction(bool addAdditionalProduction)
		{
			// arrange
			int[] documentsArtifactIds = this.controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();
			int[] productionPrecedence = addAdditionalProduction ?
				new[] { NonExistingProductionId, this.productionWithFirstDocumentId } :
				new[] { NonExistingProductionId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImagesByProductionIDsAndDocumentIDsForExport(this.TestParameters.WorkspaceId, productionPrecedence, documentsArtifactIds),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contains("Read Failed"));
			}
		}

		[IdentifiedTestCase("073fe5cc-c7f5-446c-a9fc-359e29a0949f", false)]
		[IdentifiedTestCase("36e61562-0950-4b20-a862-5669a7de1792", true)]
		public void ShouldReturnEmptyDataSetWhenRetrieveImagesByProductionIDsAndDocumentIDsForExportForNonExistingDocuments(bool addAdditionalProduction)
		{
			// arrange
			int[] productionPrecedence = addAdditionalProduction ?
				new[] { this.productionWithFirstDocumentId, this.productionWithLastDocumentId } :
				new[] { this.productionWithFirstDocumentId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImagesByProductionIDsAndDocumentIDsForExport(this.TestParameters.WorkspaceId, productionPrecedence, this.nonExistingArtifactIds);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(11), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset when documents does not exist.");
			}
		}

		[IdentifiedTest("c7a22153-d66e-4023-ae37-c617091f1dac")]
		public void ShouldRetrieveViewsByContextArtifactIDForDocumentForSearch()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveViewsByContextArtifactID(this.TestParameters.WorkspaceId, (int)ArtifactType.Document, isSearch: true);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(21), "Result should have correct number of columns");
				var actualSearchNames = new List<string>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					string name = dataRow.Field<string>(1);
					actualSearchNames.Add(name);
				}

				Assert.That(actualSearchNames, Is.Not.Empty, "It should return some searches");
				foreach (var expectedSearchName in this.searchNames)
				{
					Assert.That(actualSearchNames, Has.One.EqualTo(expectedSearchName));
				}
			}
		}

		[IdentifiedTest("408435fe-a7b7-4d19-be54-fa6b115539f3")]
		public void ShouldRetrieveViewsByContextArtifactIDForDocumentForView()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveViewsByContextArtifactID(this.TestParameters.WorkspaceId, (int)ArtifactType.Document, isSearch: false);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(20), "Result should have correct number of columns");
				var actualViewsName = new List<string>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					string name = dataRow.Field<string>(1);
					actualViewsName.Add(name);
				}

				Assert.That(actualViewsName, Is.Not.Empty, "It should return some views");
			}
		}

		[IdentifiedTest("7db35905-4db5-4643-b054-ae50b28f4928")]
		public void ShouldRetrieveViewsByContextArtifactIDForRdoForSearch()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveViewsByContextArtifactID(this.TestParameters.WorkspaceId, this.rdoWithFilesObjectTypeArtifactId, isSearch: true);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(21), "Result should have correct number of columns");
				var actualSearchName = new List<string>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					string name = dataRow.Field<string>(1);
					actualSearchName.Add(name);
				}

				Assert.That(actualSearchName, Is.Not.Empty, "It should return some searches");
			}
		}

		[IdentifiedTest("bd9cc53c-0552-4fae-8ddc-723dc4c5bfa0")]
		public void ShouldRetrieveViewsByContextArtifactIDForRdoForView()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveViewsByContextArtifactID(this.TestParameters.WorkspaceId, this.rdoWithFilesObjectTypeArtifactId, isSearch: false);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(20), "Result should have correct number of columns");
				var actualViewNames = new List<string>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					string name = dataRow.Field<string>(1);
					actualViewNames.Add(name);
				}

				Assert.That(actualViewNames, Is.Not.Empty, "It should return some views");
			}
		}

		[IdentifiedTest("f3a4d701-4a57-4858-b427-3e7ef9894eda")]
		public void ShouldReturnEmptyDataSetForRetrieveViewsByContextArtifactIDForViewWhenArtifactTypeNotExist()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveViewsByContextArtifactID(this.TestParameters.WorkspaceId, NonExistingArtifactTypeId, isSearch: false);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(20), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Empty, "It should return empty dataset");
			}
		}

		[IdentifiedTest("7465ba65-562c-4447-a4af-fbe524f77506")]
		public void ShouldReturnSearchesForRetrieveViewsByContextArtifactIDForSearchWhenArtifactTypeNotExist()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveViewsByContextArtifactID(this.TestParameters.WorkspaceId, NonExistingArtifactTypeId, isSearch: true);

				// assert
				Assert.That(actualResult.Tables[0].Columns, Has.Count.EqualTo(21), "Result should have correct number of columns");
				Assert.That(actualResult.Tables[0].Rows, Is.Not.Empty, "It should return document's saved searches");
			}
		}

		[IdentifiedTestCase("ac3939c2-2ad6-43a8-a9ca-54766f4a9cf1", false)]
		[IdentifiedTestCase("de222a2a-d9cb-47be-8fd4-d242f8cc5bb4", true)]
		public void ShouldThrowExceptionRetrieveViewsByContextArtifactIDWhenWorkspaceNotExist(bool isSearch)
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveViewsByContextArtifactID(NonExistingWorkspaceId, this.rdoWithFilesObjectTypeArtifactId, isSearch),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("fe4e3f7a-e172-464b-aaf7-a249cba4a098")]
		public void ShouldReturnTrueForIsAssociatedSearchProviderAccessibleWhenSearchIsAccessible()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				var result = sut.IsAssociatedSearchProviderAccessible(this.TestParameters.WorkspaceId, this.searchWithFirstDocumentId);

				// assert
				bool[] expectedResult = { true, true, true };
				Assert.That(result, Is.EquivalentTo(expectedResult), "Saved search is accessible");
			}
		}

		[IdentifiedTest("210e199f-445c-46fd-82c8-a99b758d826e")]
		public void ShouldReturnFalseForIsAssociatedSearchProviderAccessibleWhenSearchNotExist()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.IsAssociatedSearchProviderAccessible(this.TestParameters.WorkspaceId, NonExistingSearchId),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contains($"ArtifactID {NonExistingSearchId} does not exist"));
			}
		}

		[IdentifiedTest("bc7868a6-ef37-4dec-b6ac-4edcb1bccaf3")]
		public void ShouldThrowExceptionForIsAssociatedSearchProviderAccessibleWhenWorkspaceNotExist()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.IsAssociatedSearchProviderAccessible(NonExistingWorkspaceId, this.searchWithFirstDocumentId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTestCase("08dbf635-f83e-48c9-84af-48d15438c72b", false)]
		[IdentifiedTestCase("54ec45dc-a9a1-4a89-aa61-f72e16f28b66", true)]
		public void ShouldRetrieveAllExportableViewFields(bool isRdo)
		{
			// arrange
			int artifactTypeId = isRdo ? this.rdoWithFilesObjectTypeArtifactId : (int)ArtifactType.Document;
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				ViewFieldInfo[] result = sut.RetrieveAllExportableViewFields(this.TestParameters.WorkspaceId, artifactTypeId);

				// assert
				Assert.That(result, Is.Not.Empty, "It should returns some fields");
			}
		}

		[IdentifiedTest("8604839e-52bb-4873-b6ec-65f6ad4871c6")]
		public void ShouldReturnEmptyArrayForRetrieveAllExportableViewFieldsWhenArtifactTypeNotExist()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				ViewFieldInfo[] result = sut.RetrieveAllExportableViewFields(this.TestParameters.WorkspaceId, NonExistingArtifactTypeId);

				// assert
				Assert.That(result, Is.Empty, "It should returns empty array, because artifact type does not exist.");
			}
		}

		[IdentifiedTest("b306fdf8-32ca-49aa-9ba0-ac6bb9b5ac36")]
		public void ShouldThrowExceptionForRetrieveAllExportableViewFieldsWhenWorkspaceNotExist()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveAllExportableViewFields(NonExistingWorkspaceId, (int)ArtifactType.Document),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("61835759-2b76-4d79-842d-2c5b06676cbe")]
		public void ShouldRetrieveDefaultViewFieldIdsForProduction()
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				int[] result = sut.RetrieveDefaultViewFieldIds(
					this.TestParameters.WorkspaceId,
					this.productionWithFirstDocumentId,
					(int)ArtifactType.Document,
					isProduction: true);

				// assert
				Assert.That(result, Is.Not.Empty, "It should returns some fields");
			}
		}

		[IdentifiedTestCase("70967469-0fcb-4827-b607-8d91c8010c2e", false)]
		[IdentifiedTestCase("73bfa879-8dfa-473a-a927-96165bad5b60", true)]
		public void ShouldThrowExceptionForRetrieveDefaultViewFieldIdsWhenArtifactNotExist(bool isProduction)
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveDefaultViewFieldIds(
						this.TestParameters.WorkspaceId,
						NonExistingArtifactId,
						(int)ArtifactType.Document,
						isProduction),
					Throws.Exception.InstanceOf<NullReferenceException>());
			}
		}

		[IdentifiedTestCase("cb6b97db-dd88-489d-abbf-a94694ba9f28", false)]
		[IdentifiedTestCase("5ca87f01-edde-4883-bc41-46d5f9efbc03", true)]
		public void ShouldThrowExceptionForRetrieveDefaultViewFieldIdsWhenWorkspaceNotExist(bool isProduction)
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveDefaultViewFieldIds(
						NonExistingWorkspaceId,
						this.productionWithFirstDocumentId,
						(int)ArtifactType.Document,
						isProduction),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("0d91c313-f2d1-4af7-8ce9-3039d4777369")]
		public void ShouldRetrieveDefaultViewFieldsForIdListForProductions()
		{
			// arrange
			int[] productionsIds = { this.productionWithFirstDocumentId, this.productionWithLastDocumentId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				HybridDictionary result = sut.RetrieveDefaultViewFieldsForIdList(
					this.TestParameters.WorkspaceId,
					(int)ArtifactType.Document,
					productionsIds,
					isProductionList: true);

				// assert
				Assert.That(result, Is.Not.Empty, "It should returns some fields");
			}
		}

		[IdentifiedTestCase("918f4ea7-615d-4f11-a94c-b60749b26608", false)]
		[IdentifiedTestCase("4ebab5e5-d198-4726-a38e-194fbbef2c78", true)]
		public void ShouldReturnEmptyResultForRetrieveDefaultViewFieldsForIdListWhenArtifactNotExist(bool isProductionList)
		{
			// arrange
			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act
				HybridDictionary result = sut.RetrieveDefaultViewFieldsForIdList(
					this.TestParameters.WorkspaceId,
					(int)ArtifactType.Document,
					this.nonExistingArtifactIds,
					isProductionList);

				// assert
				Assert.That(result, Is.Empty, "It should return empty result");
			}
		}

		[IdentifiedTestCase("e01d4429-6b27-43cb-883d-1fcaad00e631", false)]
		[IdentifiedTestCase("c9ebcb4f-7d20-4e67-875d-048b13efd7fc", true)]
		public void ShouldThrowExceptionForRetrieveDefaultViewFieldsForIdListWhenWorkspaceNotExist(bool isProductionList)
		{
			// arrange
			int[] productionsIds = { this.productionWithFirstDocumentId, this.productionWithLastDocumentId };

			using (var sut = ManagerFactory.CreateSearchManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveDefaultViewFieldsForIdList(
						NonExistingWorkspaceId,
						(int)ArtifactType.Document,
						productionsIds,
						isProductionList),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		private static IEnumerable<(int documentArtifactId, string originalFileName, string filePath)> ConvertDataSetToListOfFiles(DataSet dataSet)
		{
			foreach (DataRow dataRow in dataSet.Tables[0].Rows)
			{
				int documentArtifactId = dataRow.Field<int>(2);
				string originalFileName = dataRow.Field<string>(3);
				string filePath = dataRow.Field<string>(8);

				yield return (documentArtifactId, originalFileName, filePath);
			}
		}

		private void ImportTestData()
		{
			this.originalNativeFileNames = TestData.SampleDocFiles.Select(System.IO.Path.GetFileName).ToList();
			ImportHelper.ImportDocuments(this.TestParameters);

			this.LoadDocumentsFromWorkspace();
			Assert.That(
				this.controlNumberToArtifactIdMapping,
				Has.Count.EqualTo(this.originalNativeFileNames.Count),
				"Arrange failed. Number of imported documents should be equal to number of test files");

			ImportHelper.ImportImagesForDocuments(this.TestParameters, this.controlNumberToArtifactIdMapping.Keys);
		}

		private async Task<int> CreateSavedSearchesAsync()
		{
			this.firstDocumentControlNumber = this.controlNumberToArtifactIdMapping.First().Key;
			string firstSearchName = $"Single document - {RandomHelper.NextInt(0, int.MaxValue)}";
			this.searchNames.Add(firstSearchName);
			this.searchWithFirstDocumentId = await SearchHelper.CreateSavedSearchWithSingleDocument(
												 this.TestParameters,
												 firstSearchName,
												 this.firstDocumentControlNumber).ConfigureAwait(false);

			this.lastDocumentControlNumber = this.controlNumberToArtifactIdMapping.Last().Key;
			string secondSearchName = $"Single document - {RandomHelper.NextInt(0, int.MaxValue)}";
			this.searchNames.Add(secondSearchName);
			int searchWithLastDocument = await SearchHelper.CreateSavedSearchWithSingleDocument(
											 this.TestParameters,
											 secondSearchName,
											 this.lastDocumentControlNumber).ConfigureAwait(false);
			return searchWithLastDocument;
		}

		private async Task CreateProductionsAsync(int searchWithLastDocument)
		{
			this.notProducedProductionId = await ProductionHelper.CreateProductionAsync(
											   this.TestParameters,
											   $"Not started production {RandomHelper.NextInt(0, int.MaxValue)}",
											   "NSP").ConfigureAwait(false);
			await ProductionHelper.AddDataSourceAsync(
				this.TestParameters,
				this.notProducedProductionId,
				this.searchWithFirstDocumentId).ConfigureAwait(false);

			this.productionWithFirstDocumentId = await ProductionHelper.CreateProductionAsync(
													 this.TestParameters,
													 $"Production with single document {RandomHelper.NextInt(0, int.MaxValue)}",
													 "Single").ConfigureAwait(false);
			await ProductionHelper.AddDataSourceAsync(
				this.TestParameters,
				this.productionWithFirstDocumentId,
				this.searchWithFirstDocumentId).ConfigureAwait(false);
			await ProductionHelper.StageAndRunAsync(this.TestParameters, this.productionWithFirstDocumentId)
				.ConfigureAwait(false);

			this.productionWithLastDocumentId = await ProductionHelper.CreateProductionAsync(
													this.TestParameters,
													$"Production with single document {RandomHelper.NextInt(0, int.MaxValue)}",
													"Single").ConfigureAwait(false);
			await ProductionHelper.AddDataSourceAsync(
				this.TestParameters,
				this.productionWithLastDocumentId,
				searchWithLastDocument).ConfigureAwait(false);
			await ProductionHelper.StageAndRunAsync(this.TestParameters, this.productionWithLastDocumentId)
				.ConfigureAwait(false);
		}

		private async Task CreateRdoWithFileFieldAsync()
		{
			this.rdoWithFilesObjectTypeArtifactId = await RdoHelper.CreateObjectTypeAsync(
															this.TestParameters,
															$"RdoWithFiles {RandomHelper.NextInt(0, int.MaxValue)}")
														.ConfigureAwait(false);
			this.fileFieldArtifactId = await FieldHelper.CreateFileFieldAsync(
										   this.TestParameters,
										   WellKnownFields.FilePath,
										   this.rdoWithFilesObjectTypeArtifactId).ConfigureAwait(false);
		}

		private void LoadDocumentsFromWorkspace()
		{
			IList<RelativityObject> importedDocuments = RdoHelper.QueryRelativityObjects(
				this.TestParameters,
				(int)ArtifactType.Document,
				new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });

			this.controlNumberToArtifactIdMapping = importedDocuments.Select(x => x.FieldValues).Select(
				x => new
				{
					ArtifactId = (int)x.Single(y => y.Field.Name == WellKnownFields.ArtifactId).Value,
					ControlNumber = (string)x.Single(y => y.Field.Name == WellKnownFields.ControlNumber).Value,
				}).ToDictionary(x => x.ControlNumber, x => x.ArtifactId);
		}
	}
}
