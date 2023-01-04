// ----------------------------------------------------------------------------
// <copyright file="KeplerBulkImportObjectsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service.BulkImport
{
	using System;
	using System.Data;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	using BulkImportManager = kCura.WinEDDS.Service.BulkImportManager;
	using FieldCategory = kCura.EDDS.WebAPI.BulkImportManagerBase.FieldCategory;
	using FieldType = kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType;
	using ObjectType = Relativity.DataExchange.TestFramework.RelativityHelpers.ObjectType;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi.Operations.ImportRDOs]
	public class KeplerBulkImportObjectsTests : KeplerBulkImportManagerBase
	{
		private ObjectType customObjectType;
		private RelativityObject customObjectTypeField;
		private DataTable expectedFieldValues;
		private int workspaceArtifactId;

		public KeplerBulkImportObjectsTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public new async Task OneTimeSetupAsync()
		{
			var name = $"Object-{Guid.NewGuid()}";
			this.customObjectType = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, name, this.TestParameters.WorkspaceId, RdoHelper.WorkspaceArtifactTypeId)
				                        .ConfigureAwait(false);

			this.customObjectTypeField = await FieldHelper
				                             .QueryFieldForTypeByNameAsync(
					                             this.TestParameters,
					                             this.customObjectType.TypeName,
					                             "Name").ConfigureAwait(false);

			this.workspaceArtifactId = await WorkspaceHelper.ReadRootArtifactIdAsync(this.TestParameters, this.TestParameters.WorkspaceId).ConfigureAwait(false);
		}

		[TearDown]
		public new async Task TearDownAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, this.customObjectType.ArtifactTypeId).ConfigureAwait(false);
		}

		[IdentifiedTest("1A06485D-4D25-41EC-A947-8D1D5BDB715A")]
		public async Task ShouldImportObjects()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetObjectLoadInfoAsync(NumberOfElements).ConfigureAwait(false);

				// act
				var result = sut.BulkImportObjects(this.TestParameters.WorkspaceId, loadInfo, inRepository: true);
				var hasErrors = sut.NativeRunHasErrors(this.TestParameters.WorkspaceId, loadInfo.RunID);
				var errorFileKey = sut.GenerateNonImageErrorFiles(this.TestParameters.WorkspaceId, loadInfo.RunID, this.customObjectType.ArtifactTypeId, writeHeader: true, keyFieldID: loadInfo.KeyFieldArtifactID);

				// assert
				Assert.That(result.ExceptionDetail, Is.Null, $"An error occurred when running import: {result.ExceptionDetail?.ExceptionMessage}");
				Assert.That(result.ArtifactsCreated, Is.EqualTo(NumberOfElements));
				Assert.That(result.ArtifactsUpdated, Is.EqualTo(0));
				Assert.That(result.FilesProcessed, Is.EqualTo(0));

				Assert.That(hasErrors, Is.False);

				Assert.That(errorFileKey, Is.Not.Null);
				Assert.That(errorFileKey.LogKey, Is.EqualTo(string.Empty));
				Assert.That(errorFileKey.OpticonKey, Is.Null);

				await this.AssertObjects(NumberOfElements).ConfigureAwait(false);

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("4B9B0116-3313-4D11-A722-7367AA16AD06")]
		public async Task ShouldImportZeroDocumentsWhenLoadFileIsEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetObjectLoadInfoAsync(NumberOfElements).ConfigureAwait(false);
				loadInfo.DataFileName =
					await BcpFileHelper.CreateEmptyAsync(this.TestParameters, BcpPath).ConfigureAwait(false);

				// act
				Assert.That(
					() => sut.BulkImportNative(
						this.TestParameters.WorkspaceId,
						loadInfo,
						inRepository: true,
						includeExtractedTextEncoding: false),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message.StartWith(
							@"Error: Error occured while executing 'ImportNativesStage'. Category: 'Sql', message: 'SQL Statement Failed'")
						.And.Message.Contain("Error: Invalid column name 'Name'.")
						.Or.Message.StartWith("Error: SQL Statement Failed")
						.And.Message.Contains("Error: Invalid column name 'Name'"));

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("112A20BD-3C58-481E-ABF8-3AD09F5B385E")]
		public async Task ShouldThrowWhenLoadFileNameIsEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetObjectLoadInfoAsync(NumberOfElements).ConfigureAwait(false);
				loadInfo.DataFileName = string.Empty;

				// act
				Assert.That(
					() => sut.BulkImportNative(
						this.TestParameters.WorkspaceId,
						loadInfo,
						inRepository: true,
						includeExtractedTextEncoding: false),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message.StartWith(
							@"Error: Error occured while executing 'ImportMetadataFilesToStagingTablesStage'. Category: 'Unknown', message: 'The BCP file name cannot be null or empty.")
						.And.Message.Contain("Parameter name: bulkFileName")
						.Or.Message.StartWith("Error: The BCP file name cannot be null or empty.")
						.And.Message.Contain("Parameter name: bulkFileName"));

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("230356E1-15CF-4749-A6E4-7C3A2F5F3DE1")]
		public async Task ShouldThrowWhenMappedFieldsAreEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetObjectLoadInfoAsync(NumberOfElements).ConfigureAwait(false);
				loadInfo.MappedFields = new FieldInfo[0];

				Assert.That(
					() => sut.BulkImportNative(
						this.TestParameters.WorkspaceId,
						loadInfo,
						inRepository: true,
						includeExtractedTextEncoding: false),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message.StartWith(
							@"Error: Error occured while executing 'PopulateCacheStage'. Category: 'Sql', message: 'SQL Statement Failed'")
						.And.Message.Contain("Error: Incorrect syntax near ')'")
						.Or.Message.StartWith("Error: SQL Statement Failed")
						.And.Message.Contain("Error: Incorrect syntax near ')'"));

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("2051F92F-EA6D-4271-B896-2E9BCBDC9873")]
		public async Task ShouldThrowWhenInvalidArtifactTypeId()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetObjectLoadInfoAsync(NumberOfElements).ConfigureAwait(false);
				loadInfo.ArtifactTypeID = NonExistingArtifactTypeId;

				Assert.That(
					() => sut.BulkImportNative(
						this.TestParameters.WorkspaceId,
						loadInfo,
						inRepository: true,
						includeExtractedTextEncoding: false),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>()
						.And.Message.StartWith(@"Error: Error occured while executing 'ImportNativesStage'. Category: 'Sql', message: 'SQL Statement Failed'")
						.And.Message.Contains("Error: Invalid column name 'Name'")
						.Or.Message.StartWith("Error: SQL Statement Failed")
						.And.Message.Contains("Error: Invalid column name 'Name'"));

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("55A9AB6E-D492-4FC3-8D2F-3DEB42BD72DF")]
		public async Task ShouldThrowWhenInvalidWorkspace()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetObjectLoadInfoAsync(NumberOfElements).ConfigureAwait(false);

				// act & assert
				Assert.That(
					() => sut.BulkImportObjects(NonExistingWorkspaceId, loadInfo, inRepository: true),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		private async Task<ObjectLoadInfo> GetObjectLoadInfoAsync(int numberOfArtifactsToCreate)
		{
			var fields = this.GetObjectFields(numberOfArtifactsToCreate);
			string loadFileContent = GetLoadFileContent(this.expectedFieldValues, isNative: false, parentFolderId: this.workspaceArtifactId);

			return new ObjectLoadInfo
				       {
					       ArtifactTypeID = this.customObjectType.ArtifactTypeId,
					       AuditLevel = ImportAuditLevel.FullAudit,
					       Billable = true,
					       BulkLoadFileFieldDelimiter = FieldDelimiter,
					       CodeFileName = await BcpFileHelper.CreateEmptyAsync(this.TestParameters, this.BcpPath).ConfigureAwait(false),
					       DataFileName = await BcpFileHelper.CreateAsync(this.TestParameters, loadFileContent, this.BcpPath).ConfigureAwait(false),
					       DataGridFileName = await BcpFileHelper.CreateEmptyAsync(this.TestParameters, this.BcpPath).ConfigureAwait(false),
					       DisableUserSecurityCheck = false,
					       ExecutionSource = ExecutionSource.ImportAPI,
					       KeyFieldArtifactID = this.customObjectTypeField.ArtifactID,
					       LinkDataGridRecords = false,
					       LoadImportedFullTextFromServer = false,
					       MappedFields = fields.ToArray(),
					       MoveDocumentsInAppendOverlayMode = false,
					       ObjectFileName = await BcpFileHelper.CreateEmptyAsync(this.TestParameters, this.BcpPath).ConfigureAwait(false),
					       OnBehalfOfUserToken = null,
					       Overlay = OverwriteType.Append,
					       OverlayArtifactID = -1,
					       OverlayBehavior = OverlayBehavior.UseRelativityDefaults,
					       Range = null,
					       Repository = this.DefaultFileRepository,
					       RootFolderID = 0,
					       RunID = Guid.NewGuid().ToString().Replace('-', '_'),
					       UploadFiles = false,
					       UseBulkDataImport = true,
				       };
		}

		private global::System.Collections.Generic.List<FieldInfo> GetObjectFields(int numberOfArtifactsToCreate)
		{
			global::System.Collections.Generic.List<FieldInfo> fields = new global::System.Collections.Generic.List<FieldInfo>
				                                                            {
					                                                            new FieldInfo
						                                                            {
							                                                            ArtifactID = this.customObjectTypeField.ArtifactID,
							                                                            Category = FieldCategory.Identifier,
							                                                            TextLength = DefaultFieldLength,
							                                                            DisplayName = this.customObjectTypeField.FieldValues[0].Value.ToString(),
							                                                            Type = FieldType.Varchar,
							                                                            CodeTypeID = 0,
							                                                            EnableDataGrid = false,
							                                                            FormatString = null,
							                                                            ImportBehavior = null,
							                                                            IsUnicodeEnabled = false,
						                                                            },
				                                                            };

			this.expectedFieldValues = FieldsRandomHelper.GetFieldValues(fields, numberOfArtifactsToCreate);

			return fields;
		}

		private async Task AssertObjects(int expectedNumberOfElements)
		{
			var objects = await RdoHelper.QueryRelativityObjectsAsync(
				              this.TestParameters,
				              this.customObjectType.ArtifactTypeId,
				              new[] { "Name" }).ConfigureAwait(false);
			Assert.That(objects.Count, Is.EqualTo(expectedNumberOfElements));
		}
	}
}