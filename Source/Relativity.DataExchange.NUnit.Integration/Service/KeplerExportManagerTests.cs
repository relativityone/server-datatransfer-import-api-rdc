// ----------------------------------------------------------------------------
// <copyright file="KeplerExportManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Collections.Generic;
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
	public class KeplerExportManagerTests : KeplerServiceTestBase
	{
		private const string TestUserPassword = "!4321tseT";
		private const int ResultsObjectCollectionRowsCount = 1;
		private const int ResultsObjectCollectionRowElementCount = 2;

		private string _testUserEmail = $"test.user.{Guid.NewGuid()}@test.com";
		private Dictionary<string, int> _controlNumberToArtifactIdMapping;
		private string _firstDocumentControlNumber;
		private int _testUserId;
		private int _searchWithDocumentId;
		private int _productionWithDocumentId;
		private List<string> _originalNativeFileNames;
		private int _viewArtifactId;
		private int _workspaceRootArtifactId;
		private int _workspaceGroupId;

		public KeplerExportManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task OneTimeSetUpAsync()
		{
			this._workspaceGroupId = await GroupHelper.CreateNewGroupAsync(this.TestParameters, Guid.NewGuid().ToString()).ConfigureAwait(false);
			await PermissionsHelper.AddGroupToWorkspaceAsync(this.TestParameters, this._workspaceGroupId).ConfigureAwait(false);
			await PermissionsHelper.SetWorkspaceOtherSettingsAsync(this.TestParameters, this._workspaceGroupId, new List<string>() { "Allow Import" }, true).ConfigureAwait(false);
			this._testUserId = await UsersHelper.CreateNewUserAsync(
				                   this.TestParameters,
				                   this._testUserEmail,
				                   TestUserPassword,
				                   new[] { GroupHelper.EveryoneGroupId, this._workspaceGroupId }).ConfigureAwait(false);

			this.ImportTestData();
			this._firstDocumentControlNumber = this._controlNumberToArtifactIdMapping.First().Key;
			string firstSearchName = "Single document Search";
			this._searchWithDocumentId = await SearchHelper.CreateSavedSearchWithSingleDocument(
												 this.TestParameters,
												 firstSearchName,
												 this._firstDocumentControlNumber).ConfigureAwait(false);

			this._productionWithDocumentId = await ProductionHelper.CreateProductionAsync(
				                                 this.TestParameters,
				                                 "TestProductionName",
				                                 "TestBatesProductionPrefix").ConfigureAwait(false);
			await ProductionHelper.AddDataSourceAsync(
				this.TestParameters,
				this._productionWithDocumentId,
				this._searchWithDocumentId).ConfigureAwait(false);
			await ProductionHelper.StageAndRunAsync(this.TestParameters, this._productionWithDocumentId)
				.ConfigureAwait(false);

			this._viewArtifactId = await ViewHelper.CreateViewAsync(
				                       this.TestParameters,
				                       "folderView",
				                       WellKnownArtifactTypes.DocumentArtifactTypeId,
				                       new[] { WellKnownFields.ControlNumberId }).ConfigureAwait(false);

			this._workspaceRootArtifactId = await FolderHelper.GetWorkspaceRootArtifactIdAsync(this.TestParameters).ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			await ProductionHelper.DeleteAllProductionsAsync(this.TestParameters).ConfigureAwait(false);
			await UsersHelper.RemoveUserAsync(this.TestParameters, this._testUserId).ConfigureAwait(false);
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
			await GroupHelper.RemoveGroupAsync(this.TestParameters, this._workspaceGroupId).ConfigureAwait(false);
		}

		[Test]
		[IdentifiedTest("50D65BA8-357A-40F3-BD2C-388530EF60C8")]
		public void ShouldReturnTrueWhenHasPermissionForHasExportPermissions()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				bool actualResult = sut.HasExportPermissions(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(actualResult, Is.True);
			}
		}

		[Test]
		[IdentifiedTest("CD76562E-7C0D-4805-9133-1C53895C355C")]
		public void ShouldReturnFalseWhenHasNoPermissionForHasExportPermissions()
		{
			// arrange
			var user = this.Credential.UserName;
			var pass = this.Credential.Password;
			this.Credential.UserName = this._testUserEmail;
			this.Credential.Password = TestUserPassword;

			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				bool actualResult = sut.HasExportPermissions(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(actualResult, Is.False);
			}

			this.Credential.UserName = user;
			this.Credential.Password = pass;
		}

		[Test]
		[IdentifiedTest("F630A92C-FCF9-4AAC-8479-7AECF10922FB")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForHasExportPermissions()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.HasExportPermissions(NonExistingWorkspaceId),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contain("Could not retrieve ApplicationID #0"));
			}
		}

		[Test]
		[IdentifiedTestCase("4B5E53EA-E1BC-4D91-A280-F0A63C077D1D", 0)]
		[IdentifiedTestCase("0EA78DCF-0FA8-4B66-9CFE-341C2A46CEB1", -1)]
		[IdentifiedTestCase("28CB5819-AB8B-4747-81BB-F8C3027DEFD3", 1)]
		public void ShouldReturnInitializationResultForInitializeSearchExport(int startRow)
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults actualResult = sut.InitializeSearchExport(
					this.TestParameters.WorkspaceId,
					this._searchWithDocumentId,
					Array.Empty<int>(),
					startRow);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.ColumnNames.Contains("ControlNumber"), Is.True);
				Assert.That(actualResult.ColumnNames.Contains("ArtifactID"), Is.True);
				Assert.That(actualResult.RowCount, Is.EqualTo(1));
			}
		}

		[Test]
		[IdentifiedTest("46329F04-FD7D-4E2C-AA2A-62879D478E19")]
		public void ShouldThrowExceptionWhenNoSearchArtifactIdForInitializeSearchExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.InitializeSearchExport(
						this.TestParameters.WorkspaceId,
						NonExistingSearchId,
						Array.Empty<int>(),
						0),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contain("ArtifactID 0 does not exist."));
			}
		}

		[Test]
		[IdentifiedTest("80BF1D61-B403-4CA3-B864-13BB9763545E")]
		public void ShouldReturnsResultWhenAvailableViewFieldIsNullForInitializeSearchExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults actualResult = sut.InitializeSearchExport(
					this.TestParameters.WorkspaceId,
					this._searchWithDocumentId,
					null,
					0);

				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.ColumnNames.Contains("ControlNumber"), Is.True);
				Assert.That(actualResult.ColumnNames.Contains("ArtifactID"), Is.True);
				Assert.That(actualResult.RowCount, Is.EqualTo(1));
			}
		}

		[Test]
		[IdentifiedTest("1FD68C15-0F17-4316-946A-D1622C4B62D7")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForInitializeSearchExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.InitializeSearchExport(NonExistingWorkspaceId, this._searchWithDocumentId, Array.Empty<int>(), 0),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[Test]
		[IdentifiedTest("AA718B4E-ABF1-44BF-86A8-E03A970A9661")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForInitializeProductionExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.InitializeProductionExport(NonExistingWorkspaceId, NonExistingProductionId, Array.Empty<int>(), 0),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[Test]
		[IdentifiedTest("614FF365-CA31-4BE1-99FF-7DE1F8AE3ABB")]
		public void ShouldThrowExceptionWhenProductionDoesNotExistForInitializeProductionExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.InitializeProductionExport(this.TestParameters.WorkspaceId, NonExistingProductionId, Array.Empty<int>(), 0),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contain("ArtifactID 0 does not exist."));
			}
		}

		[Test]
		[IdentifiedTest("D30EF63F-E9E6-4FC4-BFDB-47DC5950F63E")]
		public void ShouldReturnResultForInitializeProductionExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults actualResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.ColumnNames.Contains("ControlNumber"), Is.True);
				Assert.That(actualResult.ColumnNames.Contains("ArtifactID"), Is.True);
				Assert.That(actualResult.RowCount, Is.EqualTo(1));
			}
		}

		[Test]
		[IdentifiedTest("731DDD0F-C438-41D2-977B-1953F8F66728")]
		public void ShouldReturnResultWhenAvailableViewFieldIsNullForInitializeProductionExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.InitializeProductionExport(this.TestParameters.WorkspaceId, this._productionWithDocumentId, null, 0),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contain("Value cannot be null."));
			}
		}

		[Test]
		[IdentifiedTestCase("AFFD5005-8CE6-413D-A637-48249052B6EA", 0)]
		[IdentifiedTestCase("8EBD6180-CB66-42A3-BD6E-BD52F0484C15", 1)]
		[IdentifiedTestCase("55B1E9C6-DACD-4BE8-B1E1-19D4C357F6A0", -1)]
		public void ShouldReturnResultWithDifferentRowStartForInitializeProductionExport(int startRow)
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults actualResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					startRow);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.ColumnNames.Contains("ControlNumber"), Is.True);
				Assert.That(actualResult.ColumnNames.Contains("ArtifactID"), Is.True);
				Assert.That(actualResult.RowCount, Is.EqualTo(1));
			}
		}

		[Test]
		[IdentifiedTest("8419771B-B3DC-486D-9D99-DA075E7FA2CE")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForInitializeFolderExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.InitializeFolderExport(NonExistingWorkspaceId, 0, 0, false, Array.Empty<int>(), 0, 0),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[Test]
		[IdentifiedTest("E53B2C5B-88BB-49CE-A68B-3609950052FE")]
		public void ShouldReturnFoldersForInitializeFolderExport()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults actualResult = sut.InitializeFolderExport(
					this.TestParameters.WorkspaceId,
					this._viewArtifactId,
					this._workspaceRootArtifactId,
					true,
					Array.Empty<int>(),
					0,
					WellKnownArtifactTypes.DocumentArtifactTypeId);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.ColumnNames.Contains("ControlNumber"), Is.True);
				Assert.That(actualResult.ColumnNames.Contains("ArtifactID"), Is.True);
				Assert.That(actualResult.RowCount, Is.EqualTo(10));
			}
		}

		[Test]
		[IdentifiedTestCase("EF492319-F2AB-468E-9BBD-7A9203DF8D00", true)]
		[IdentifiedTestCase("87396931-9688-4317-9B17-0B2DEDF3649E", false)]
		public void ShouldReturnObjectForRetrieveResultsBlockForProductionStartingFromIndex(bool diplayMulticodesAsNested)
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act
				object actualResult = sut.RetrieveResultsBlockForProductionStartingFromIndex(
					this.TestParameters.WorkspaceId,
					initializationResult.RunId,
					WellKnownArtifactTypes.DocumentArtifactTypeId,
					Array.Empty<int>(),
					10,
					diplayMulticodesAsNested,
					';',
					']',
					Array.Empty<int>(),
					this._productionWithDocumentId,
					0);

				// assert
				this.ThenResultsObjectIsSize(actualResult);
			}
		}

		[Test]
		[IdentifiedTest("A195E4F3-ADF3-4C3A-88EA-B6C399422206")]
		public void ShouldThrowExceptionWhenNegativeChunkSizeForProductionStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act && assert
				Assert.That(
					() => sut.RetrieveResultsBlockForProductionStartingFromIndex(
						this.TestParameters.WorkspaceId,
						initializationResult.RunId,
						WellKnownArtifactTypes.DocumentArtifactTypeId,
						Array.Empty<int>(),
						-10,
						true,
						';',
						']',
						Array.Empty<int>(),
						this._productionWithDocumentId,
						0),
					Throws.Exception
						.InstanceOf<SoapException>()
						.With.Message.Contains("SQL Statement Failed"));
			}
		}

		[Test]
		[IdentifiedTest("8C48E054-C45A-4E41-BBA8-454A6872A596")]
		public void ShouldThrowExceptionWhenWrongRunIdForProductionStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.RetrieveResultsBlockForProductionStartingFromIndex(
						this.TestParameters.WorkspaceId,
						Guid.NewGuid(),
						WellKnownArtifactTypes.DocumentArtifactTypeId,
						Array.Empty<int>(),
						10,
						true,
						';',
						']',
						Array.Empty<int>(),
						this._productionWithDocumentId,
						0),
					Throws.Exception
						.InstanceOf<SoapException>()
						.With.Message.Contains("SQL Statement Failed"));
			}
		}

		[Test]
		[IdentifiedTest("546AD3BD-28B4-4A20-806C-2DD4BFC126FD")]
		public void ShouldThrowExceptionWhenWrongTypeIdForProductionStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act && assert
				Assert.That(
					() => sut.RetrieveResultsBlockForProductionStartingFromIndex(
						this.TestParameters.WorkspaceId,
						initializationResult.RunId,
						-1,
						Array.Empty<int>(),
						10,
						true,
						';',
						']',
						Array.Empty<int>(),
						this._productionWithDocumentId,
						0),
					Throws.Exception
						.InstanceOf<SoapException>()
						.With.Message.Contains("The given key was not present in the dictionary."));
			}
		}

		[Test]
		[IdentifiedTest("158D85C1-8B9D-4B4F-89F6-57926EB80F8B")]
		public void ShouldThrowExceptionWhenWrongProductionIdForProductionStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act && assert
				Assert.That(
					() => sut.RetrieveResultsBlockForProductionStartingFromIndex(
						this.TestParameters.WorkspaceId,
						initializationResult.RunId,
						WellKnownArtifactTypes.DocumentArtifactTypeId,
						Array.Empty<int>(),
						10,
						true,
						';',
						']',
						Array.Empty<int>(),
						NonExistingProductionId,
						0),
					Throws.Exception
						.InstanceOf<SoapException>()
						.With.Message.Contains("Object reference not set to an instance of an object."));
			}
		}

		[Test]
		[IdentifiedTest("AE74120D-E088-45A5-83BD-B3BFF569408B")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForRetrieveResultsBlockForProductionStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act
				Assert.That(
					() => sut.RetrieveResultsBlockForProductionStartingFromIndex(
						NonExistingWorkspaceId,
						initializationResult.RunId,
						WellKnownArtifactTypes.DocumentArtifactTypeId,
						Array.Empty<int>(),
						10,
						true,
						';',
						']',
						Array.Empty<int>(),
						this._productionWithDocumentId,
						0),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[Test]
		[IdentifiedTestCase("9E3E83C0-EC63-476E-8F92-BA98661518D2", true)]
		[IdentifiedTestCase("6E569CBF-7B48-4A79-80D5-73C1B0CF43DE", false)]
		public void ShouldReturnObjectForRetrieveResultsBlockForRetrieveResultsBlockStartingFromIndex(bool diplayMulticodesAsNested)
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act
				object actualResult = sut.RetrieveResultsBlockStartingFromIndex(
					this.TestParameters.WorkspaceId,
					initializationResult.RunId,
					WellKnownArtifactTypes.DocumentArtifactTypeId,
					Array.Empty<int>(),
					10,
					diplayMulticodesAsNested,
					';',
					']',
					Array.Empty<int>(),
					0);

				// assert
				this.ThenResultsObjectIsSize(actualResult);
			}
		}

		[Test]
		[IdentifiedTest("F3B8A393-D982-49A2-BEB5-9B10ECBE74B4")]
		public void ShouldThrowExceptionWhenNegativeChunkSizeForRetrieveResultsBlockStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act && assert
				Assert.That(
					() => sut.RetrieveResultsBlockStartingFromIndex(
						this.TestParameters.WorkspaceId,
						initializationResult.RunId,
						WellKnownArtifactTypes.DocumentArtifactTypeId,
						Array.Empty<int>(),
						-10,
						true,
						';',
						']',
						Array.Empty<int>(),
						0),
					Throws.Exception
						.InstanceOf<SoapException>()
						.With.Message.Contains("SQL Statement Failed"));
			}
		}

		[Test]
		[IdentifiedTest("94F5ADBF-79FD-4B09-B954-B230DA3F0779")]
		public void ShouldThrowExceptionWhenWrongRunIdForRetrieveResultsBlockStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.RetrieveResultsBlockStartingFromIndex(
						this.TestParameters.WorkspaceId,
						Guid.NewGuid(),
						WellKnownArtifactTypes.DocumentArtifactTypeId,
						Array.Empty<int>(),
						10,
						true,
						';',
						']',
						Array.Empty<int>(),
						0),
					Throws.Exception
						.InstanceOf<SoapException>()
						.With.Message.Contains("SQL Statement Failed"));
			}
		}

		[Test]
		[IdentifiedTest("71929089-3D4B-4037-B79D-A151E881C9A1")]
		public void ShouldThrowExceptionWhenWrongTypeIdForRetrieveResultsBlockStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act && assert
				Assert.That(
					() => sut.RetrieveResultsBlockStartingFromIndex(
						this.TestParameters.WorkspaceId,
						initializationResult.RunId,
						-1,
						Array.Empty<int>(),
						10,
						true,
						';',
						']',
						Array.Empty<int>(),
						0),
					Throws.Exception
						.InstanceOf<SoapException>()
						.With.Message.Contains("The given key was not present in the dictionary"));
			}
		}

		[Test]
		[IdentifiedTest("9E41A666-10DC-4A05-893E-9AB37F76AA1B")]
		public void ShouldThrowExceptionWhenWrongProductionIdForRetrieveResultsBlockStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act
				object actualResult = sut.RetrieveResultsBlockStartingFromIndex(
					this.TestParameters.WorkspaceId,
					initializationResult.RunId,
					WellKnownArtifactTypes.DocumentArtifactTypeId,
					Array.Empty<int>(),
					10,
					true,
					';',
					']',
					Array.Empty<int>(),
					0);

				// assert
				this.ThenResultsObjectIsSize(actualResult);
			}
		}

		[Test]
		[IdentifiedTest("52555CCA-C4D1-45B0-91EC-FE3DC150E065")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForRetrieveResultsBlockForRetrieveResultsBlockStartingFromIndex()
		{
			// arrange
			using (IExportManager sut = ManagerFactory.CreateExportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults initializationResult = sut.InitializeProductionExport(
					this.TestParameters.WorkspaceId,
					this._productionWithDocumentId,
					Array.Empty<int>(),
					0);

				// act
				Assert.That(
					() => sut.RetrieveResultsBlockStartingFromIndex(
						NonExistingWorkspaceId,
						initializationResult.RunId,
						WellKnownArtifactTypes.DocumentArtifactTypeId,
						Array.Empty<int>(),
						10,
						true,
						';',
						']',
						Array.Empty<int>(),
						0),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		private void ImportTestData()
		{
			this._originalNativeFileNames = TestData.SampleDocFiles.Select(System.IO.Path.GetFileName).ToList();
			ImportHelper.ImportDocuments(this.TestParameters);

			this.LoadDictionaryOfDocumentsFromWorkspace();
			Assert.That(
				this._controlNumberToArtifactIdMapping,
				Has.Count.EqualTo(this._originalNativeFileNames.Count),
				"Arrange failed. Number of imported documents should be equal to number of test files");

			ImportHelper.ImportImagesForDocuments(this.TestParameters, this._controlNumberToArtifactIdMapping.Keys);
		}

		private void LoadDictionaryOfDocumentsFromWorkspace()
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

		private void ThenResultsObjectIsSize(object actualResult)
		{
			Assert.That(actualResult, Is.Not.Null);
			int firstDimensionLength;
			int secondDimensionLength;
			if (this.UseKepler)
			{
				Assert.That(actualResult, Is.TypeOf<object[][]>());
				object[][] parsedValue = actualResult as object[][];
				firstDimensionLength = parsedValue.Length;
				secondDimensionLength = parsedValue[0].Length;
			}
			else
			{
				Assert.That(actualResult, Is.TypeOf<object[]>());
				object[] parsedValue = actualResult as object[];
				Assert.That(parsedValue[0], Is.TypeOf<object[]>());
				firstDimensionLength = parsedValue.Length;
				object[] secondParsedValue = parsedValue[0] as object[];
				secondDimensionLength = secondParsedValue.Length;
			}

			Assert.That(firstDimensionLength, Is.EqualTo(ResultsObjectCollectionRowsCount));
			Assert.That(secondDimensionLength, Is.EqualTo(ResultsObjectCollectionRowElementCount));
		}
	}
}