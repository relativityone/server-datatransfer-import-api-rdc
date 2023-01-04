// ----------------------------------------------------------------------------
// <copyright file="KeplerBulkImportNativesTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service.BulkImport
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Testing.Identification;

	using BulkImportManager = kCura.WinEDDS.Service.BulkImportManager;
	using FieldCategory = kCura.EDDS.WebAPI.BulkImportManagerBase.FieldCategory;
	using FieldType = kCura.EDDS.WebAPI.BulkImportManagerBase.FieldType;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class KeplerBulkImportNativesTests : KeplerBulkImportManagerBase
	{
		private const int NumberOfCustomFields = 10;

		private readonly Dictionary<string, int> customFieldsDictionary = new Dictionary<string, int>();

		private DataTable expectedFieldValues;
		private List<string> expectedFieldsNames;

		public KeplerBulkImportNativesTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public new async Task OneTimeSetupAsync()
		{
			for (int i = 0; i < NumberOfCustomFields; i++)
			{
				var field = await FieldHelper.CreateFixedLengthTextFieldAsync(
					            this.TestParameters,
					            DocumentObjectTypeId,
					            $"Field_{i}",
					            isOpenToAssociations: true,
					            length: DefaultFieldLength).ConfigureAwait(false);
				this.customFieldsDictionary.Add($"Field_{i}", field);
			}
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			foreach (var customFieldId in this.customFieldsDictionary.Values)
			{
				await FieldHelper.DeleteFieldAsync(this.TestParameters, customFieldId).ConfigureAwait(false);
			}
		}

		[IdentifiedTest("940C2DCA-9B49-4DEE-AEFC-B1EFCF2362BF")]
		public async Task ShouldImportDocuments()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetNativeLoadInfoAsync(NumberOfElements).ConfigureAwait(false);

				// act
				var result = sut.BulkImportNative(this.TestParameters.WorkspaceId, loadInfo, inRepository: true, includeExtractedTextEncoding: false);
				var hasErrors = sut.NativeRunHasErrors(this.TestParameters.WorkspaceId, loadInfo.RunID);
				var errorFileKey = sut.GenerateNonImageErrorFiles(this.TestParameters.WorkspaceId, loadInfo.RunID, DocumentObjectTypeId, writeHeader: true, keyFieldID: loadInfo.KeyFieldArtifactID);

				// assert
				Assert.That(result.ExceptionDetail, Is.Null, $"An error occurred when running import: {result.ExceptionDetail?.ExceptionMessage}");
				Assert.That(result.ArtifactsCreated, Is.EqualTo(NumberOfElements));
				Assert.That(result.ArtifactsUpdated, Is.EqualTo(0));
				Assert.That(result.FilesProcessed, Is.EqualTo(0));

				Assert.That(hasErrors, Is.False);

				Assert.That(errorFileKey, Is.Not.Null);
				Assert.That(errorFileKey.LogKey, Is.EqualTo(string.Empty));
				Assert.That(errorFileKey.OpticonKey, Is.Null);

				await this.AssertDocuments(NumberOfElements).ConfigureAwait(false);

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("5A183C8D-DBA1-4DB8-A34D-82A6475A1495")]
		public async Task ShouldImportZeroDocumentsWhenLoadFileIsEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetNativeLoadInfoAsync(NumberOfElements).ConfigureAwait(false);
				loadInfo.DataFileName =
					await BcpFileHelper.CreateEmptyAsync(this.TestParameters, BcpPath).ConfigureAwait(false);

				// act
				var result = sut.BulkImportNative(this.TestParameters.WorkspaceId, loadInfo, inRepository: true, includeExtractedTextEncoding: false);
				var hasErrors = sut.NativeRunHasErrors(this.TestParameters.WorkspaceId, loadInfo.RunID);
				var errorFileKey = sut.GenerateNonImageErrorFiles(this.TestParameters.WorkspaceId, loadInfo.RunID, DocumentObjectTypeId, writeHeader: true, keyFieldID: loadInfo.KeyFieldArtifactID);

				// assert
				AssertThereIsNoDocumentProcessedAndNoErrors(result, hasErrors, errorFileKey);

				await this.AssertDocuments(0).ConfigureAwait(false);

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("6ABA569F-4C5D-4691-B04B-81263C81F137")]
		public async Task ShouldThrowWhenLoadFileNameIsEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetNativeLoadInfoAsync(NumberOfElements).ConfigureAwait(false);
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
						.Or.Message.StartWith("Error: The BCP file name cannot be null or empty"));

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("ACD3CC76-D2E0-4957-A81F-72DCB7F758ED")]
		public async Task ShouldThrowWhenMappedFieldsAreEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var loadInfo = await this.GetNativeLoadInfoAsync(NumberOfElements).ConfigureAwait(false);
				loadInfo.MappedFields = new FieldInfo[0];

				Assert.That(
					() => sut.BulkImportNative(
						this.TestParameters.WorkspaceId,
						loadInfo,
						inRepository: true,
						includeExtractedTextEncoding: false),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message.StartWith(
							@"Error: Error occured while executing 'PopulateCacheStage'. Category: 'Sql', message: 'SQL Statement Failed'")
						.Or.Message.StartWith("Error: SQL Statement Failed"));

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
				var loadInfo = await this.GetNativeLoadInfoAsync(NumberOfElements).ConfigureAwait(false);

				// act & assert
				Assert.That(
					() => sut.BulkImportNative(NonExistingWorkspaceId, loadInfo, inRepository: true, includeExtractedTextEncoding: false),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		private async Task<NativeLoadInfo> GetNativeLoadInfoAsync(int numberOfArtifactsToCreate)
		{
			var fields = this.GetNativeFields(numberOfArtifactsToCreate);
			string loadFileContent = GetLoadFileContent(this.expectedFieldValues, isNative: true, parentFolderId: WorkspaceRootFolderId);

			return new NativeLoadInfo
				       {
					       AuditLevel = ImportAuditLevel.FullAudit,
					       Billable = true,
					       BulkLoadFileFieldDelimiter = FieldDelimiter,
					       CodeFileName = await BcpFileHelper.CreateEmptyAsync(this.TestParameters, this.BcpPath).ConfigureAwait(false),
					       DataFileName = await BcpFileHelper.CreateAsync(this.TestParameters, loadFileContent, this.BcpPath).ConfigureAwait(false),
					       DataGridFileName = await BcpFileHelper.CreateEmptyAsync(this.TestParameters, this.BcpPath).ConfigureAwait(false),
					       DisableUserSecurityCheck = false,
					       ExecutionSource = ExecutionSource.ImportAPI,
					       KeyFieldArtifactID = this.DocumentIdentifier.ArtifactID,
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
					       RootFolderID = WorkspaceRootFolderId,
					       RunID = Guid.NewGuid().ToString().Replace('-', '_'),
					       UploadFiles = false,
					       UseBulkDataImport = true,
				       };
		}

		private async Task AssertDocuments(int expectedNumberOfElements)
		{
			var objects = await RdoHelper.QueryRelativityObjectsAsync(
				              this.TestParameters,
				              DocumentObjectTypeId,
				              this.expectedFieldsNames).ConfigureAwait(false);
			Assert.That(objects.Count, Is.EqualTo(expectedNumberOfElements));
			ThenTheFieldsHaveCorrectValues(this.expectedFieldValues, objects);
		}

		private List<FieldInfo> GetNativeFields(int numberOfArtifactsToCreate)
		{
			List<FieldInfo> fields = new List<FieldInfo> { this.GetIdentifierField() };

			foreach (var field in this.customFieldsDictionary)
			{
				fields.Add(
					new FieldInfo()
						{
							ArtifactID = field.Value,
							Category = FieldCategory.AutoCreate,
							CodeTypeID = 0,
							DisplayName = field.Key,
							EnableDataGrid = false,
							FormatString = null,
							ImportBehavior = null,
							IsUnicodeEnabled = false,
							TextLength = DefaultFieldLength,
							Type = FieldType.Varchar,
						});
			}

			this.expectedFieldValues = FieldsRandomHelper.GetFieldValues(fields, numberOfArtifactsToCreate);

			this.expectedFieldsNames = fields.Select(x => x.DisplayName).ToList();
			return fields;
		}

		private FieldInfo GetIdentifierField()
		{
			return new FieldInfo
				       {
					       ArtifactID = this.DocumentIdentifier.ArtifactID,
					       Category = FieldCategory.Identifier,
					       Type = FieldType.Varchar,
					       DisplayName = this.DocumentIdentifier.FieldValues[0].Value.ToString(),
					       TextLength = DefaultFieldLength,
					       CodeTypeID = 0,
					       EnableDataGrid = false,
					       FormatString = null,
					       ImportBehavior = null,
					       IsUnicodeEnabled = false,
				       };
		}
	}
}