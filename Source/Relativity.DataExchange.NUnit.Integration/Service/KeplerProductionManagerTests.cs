// ----------------------------------------------------------------------------
// <copyright file="KeplerProductionManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Services.Protocols;
    using global::NUnit.Framework;

    using kCura.WinEDDS.Service;
    using kCura.WinEDDS.Service.Replacement;
    using Relativity.DataExchange.TestFramework;
    using Relativity.DataExchange.TestFramework.RelativityHelpers;
    using Relativity.Services.Objects.DataContracts;
    using Relativity.Testing.Identification;

    [TestFixture(true)]
    [TestFixture(false)]
    [Feature.DataTransfer.RelativityDesktopClient.Export]
    public class KeplerProductionManagerTests : KeplerServiceTestBase
	{
		private const int NonExistingWorkspaceID = 0;
		private const int NonExistingProductionArtifactID = -15;

		private const string TestProductionName = "TestProduction";
		private const string EligibleTestProductionName = "EligibleTestProduction";
		private const string TestBatesProductionPrefix = "TestPrefix";

		private readonly List<string> _searchNames = new List<string>();
		private int _testProductionArtifactId;
		private int _testEligibleProductionArtifactId;
		private Dictionary<string, int> _controlNumberToArtifactIdMapping;
		private string _firstDocumentControlNumber;
		private List<string> _originalNativeFileNames;

		private int _searchWithFirstDocumentId;

		public KeplerProductionManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task KeplerFieldManagerTestsOneTimeSetup()
		{
			this.ImportTestData();
			await this.CreateSavedSearchesAsync().ConfigureAwait(false);
			await this.CreateProductionsAsync().ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task KeplerFieldManagerTestsOneTimeTearDownAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
			await ProductionHelper.DeleteAllProductionsAsync(this.TestParameters).ConfigureAwait(false);
		}

		[IdentifiedTest("1F393303-9BE2-4F0E-B348-68C450DF3FDB")]
		public void ShouldReturnCorrectValueForRead()
		{
			// arrange
			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo actualResult = sut.Read(this.TestParameters.WorkspaceId, this._testProductionArtifactId);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.Name, Is.EqualTo(TestProductionName));
			}
		}

		[IdentifiedTest("F11E55F7-7B75-4550-988A-38F55ABC646C")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForRead()
		{
			// arrange
			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.Read(NonExistingWorkspaceID, this._testProductionArtifactId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		[IdentifiedTest("A9FE2CF4-1478-4DC9-B3DC-CE6ECE156101")]
		public void ShouldTrowExceptionWhenProductionDoesNotExistForRead()
		{
			// arrange
			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				Assert.Catch<SoapException>(() => sut.Read(this.TestParameters.WorkspaceId, NonExistingProductionArtifactID));
			}
		}

		[IdentifiedTest("95E42DDE-1D28-420E-8A94-9A57AE86094D")]
		public void ShouldReturnSingleResultForRetrieveProducedByContextArtifactID()
		{
			// arrange
			string filterStringArtifactId = $"ArtifactID = {this._testProductionArtifactId}";

			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveProducedByContextArtifactID(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(actualResult.Tables[0].Rows.Count, Is.EqualTo(1));
				DataRow dataRow = actualResult.Tables[0].Select(filterStringArtifactId)[0];
				Assert.That(dataRow["ArtifactId"], Is.EqualTo(this._testProductionArtifactId));
				Assert.That(dataRow["Name"], Is.EqualTo(TestProductionName));
				Assert.That(dataRow["Prefix"], Is.EqualTo(TestBatesProductionPrefix));
			}
		}

		[IdentifiedTest("F8D761C5-F07F-460B-82F5-8C6B20893AE1")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForRetrieveProducedByContextArtifactID()
		{
			// arrange
			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveProducedByContextArtifactID(NonExistingWorkspaceID),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		[IdentifiedTest("4AC25C49-66D4-45A3-B267-7609ABDF561A")]
		public void ShouldReturnSingleResultForRetrieveImportEligibleByContextArtifactID()
		{
			// arrange
			string filterStringArtifactId = $"ArtifactID = {this._testEligibleProductionArtifactId}";

			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveImportEligibleByContextArtifactID(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(actualResult.Tables[0].Rows, Is.Not.Empty);
				Assert.That(actualResult.Tables[0].Select(filterStringArtifactId).Length, Is.EqualTo(1));
			}
		}

		[IdentifiedTest("AEA235CB-9AE5-4D2C-A296-F216859D0912")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForRetrieveImportEligibleByContextArtifactID()
		{
			// arrange
			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveImportEligibleByContextArtifactID(NonExistingWorkspaceID),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		[IdentifiedTest("2C5612B4-BAA1-4599-9422-62E05F290780")]
		public void ShouldReturnObjectForRetrieveBatesByProductionAndDocument()
		{
			// arrange
			int[] productionsIds = new[] { this._testProductionArtifactId };
			int[] documentsIds = this._controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				object[][] actualResponse = sut.RetrieveBatesByProductionAndDocument(this.TestParameters.WorkspaceId, productionsIds, documentsIds);

				// assert
				Assert.That(actualResponse, Is.Not.Empty);
				Assert.That(actualResponse.Length, Is.EqualTo(1));
				Assert.That(actualResponse[0].Length, Is.EqualTo(3));
			}
		}

		[IdentifiedTest("0F3C4222-1192-48BE-B19B-58B78DAFE6E1")]
		public void ShouldReturnEmptyWhenNonExistingProductionForRetrieveBatesByProductionAndDocument()
		{
			// arrange
			int[] productionsIds = new[] { NonExistingProductionArtifactID };
			int[] documentsIds = this._controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				object[][] actualResult = sut.RetrieveBatesByProductionAndDocument(this.TestParameters.WorkspaceId, productionsIds, documentsIds);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult, Is.Empty);
			}
		}

		[IdentifiedTest("FC9399C2-7E9A-4A1E-91D3-923CE8B357F9")]
		public void ShouldReturnEmptyWhenNonExistingDocumentsForRetrieveBatesByProductionAndDocument()
		{
			// arrange
			int[] productionsIds = new[] { this._testProductionArtifactId };
			int[] documentsIds = new[] { NonExistingProductionArtifactID };

			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				object[][] actualResult = sut.RetrieveBatesByProductionAndDocument(this.TestParameters.WorkspaceId, productionsIds, documentsIds);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult, Is.Empty);
			}
		}

		[IdentifiedTest("2C5612B4-BAA1-4599-9422-62E05F290780")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForRetrieveBatesByProductionAndDocument()
		{
			// arrange
			int[] productionsIds = new[] { this._testProductionArtifactId };
			int[] documentsIds = this._controlNumberToArtifactIdMapping.Select(x => x.Value).ToArray();

			using (IProductionManager sut = ManagerFactory.CreateProductionManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveBatesByProductionAndDocument(NonExistingWorkspaceID, productionsIds, documentsIds),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		private void ImportTestData()
		{
			this._originalNativeFileNames = TestData.SampleDocFiles.Select(System.IO.Path.GetFileName).ToList();
			ImportHelper.ImportDocuments(this.TestParameters);

			this.LoadDocumentsFromWorkspace();
			Assert.That(
				this._controlNumberToArtifactIdMapping,
				Has.Count.EqualTo(this._originalNativeFileNames.Count),
				"Arrange failed. Number of imported documents should be equal to number of test files");

			ImportHelper.ImportImagesForDocuments(this.TestParameters, this._controlNumberToArtifactIdMapping.Keys);
		}

		private void LoadDocumentsFromWorkspace()
		{
			IList<RelativityObject> importedDocuments = RdoHelper.QueryRelativityObjects(
				this.TestParameters,
				(int)ArtifactType.Document,
				new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber });

			this._controlNumberToArtifactIdMapping = importedDocuments.Select(x => x.FieldValues).Select(
				x => new
					     {
						     ArtifactId = (int)x.Single(y => y.Field.Name == WellKnownFields.ArtifactId).Value,
						     ControlNumber = (string)x.Single(y => y.Field.Name == WellKnownFields.ControlNumber).Value,
					     }).ToDictionary(x => x.ControlNumber, x => x.ArtifactId);
		}

		private async Task CreateSavedSearchesAsync()
		{
			this._firstDocumentControlNumber = this._controlNumberToArtifactIdMapping.First().Key;
			string firstSearchName = $"Single document - {RandomHelper.NextInt(0, int.MaxValue)}";
			this._searchNames.Add(firstSearchName);
			this._searchWithFirstDocumentId = await SearchHelper.CreateSavedSearchWithSingleDocument(
												 this.TestParameters,
												 firstSearchName,
												 this._firstDocumentControlNumber).ConfigureAwait(false);
		}

		private async Task CreateProductionsAsync()
		{
			this._testEligibleProductionArtifactId = await ProductionHelper.CreateProductionAsync(
				                                         this.TestParameters,
				                                         EligibleTestProductionName,
				                                         TestBatesProductionPrefix).ConfigureAwait(false);

			this._testProductionArtifactId = await ProductionHelper.CreateProductionAsync(
				                                 this.TestParameters,
				                                 TestProductionName,
				                                 TestBatesProductionPrefix).ConfigureAwait(false);
			await ProductionHelper.AddDataSourceAsync(
				this.TestParameters,
				this._testProductionArtifactId,
				this._searchWithFirstDocumentId).ConfigureAwait(false);
			await ProductionHelper.StageAndRunAsync(this.TestParameters, this._testProductionArtifactId)
				.ConfigureAwait(false);
		}
	}
}