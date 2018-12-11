using kCura.NUnit.Integration;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using kCura.Relativity.ImportAPI.IntegrationTests.Services;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Data;
using System.Threading.Tasks;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	[TestFixture]
	public class DuplicateFieldImportTest : TestBase
	{
		private int _imagingProfileObjectTypeId;
		private int _imagingProfileIdentityFieldId;

		// The object type under test.
		private const string _IMAGING_PROFILE_OBJECT_TYPE_NAME = "Imaging Profile";
		private const string _IMAGING_PROFILE_NAME_FIELD = "Name";		
		private const string _IMAGING_PROFILE_RESTRICTED_NATIVES_FIELD = "Restricted Native Types";

		private readonly string _name = Guid.NewGuid().ToString();
		private readonly string _associatedTypeDuplicateName = Guid.NewGuid().ToString();

		// Multi-object Guids
		private static readonly Guid _NATIVE_TYPE_OBJECT_TYPE_GUID = Guid.Parse("FAB01339-E3AF-4B3C-B547-2D3AF9DA5800");
		private static readonly Guid _NATIVE_TYPE_NAME_FIELD_GUID = Guid.Parse("3334FA5A-6138-4ADC-BBA1-57E3A5904623");

		[OneTimeSetUp]
		public override void OneTimeSetUp()
		{
			base.OneTimeSetUp();
			_imagingProfileObjectTypeId = ObjectTypeService.GetArtifactTypeId(WorkspaceId, _IMAGING_PROFILE_OBJECT_TYPE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();
			_imagingProfileIdentityFieldId = FieldService.GetIdentifierFieldId(WorkspaceId, _IMAGING_PROFILE_OBJECT_TYPE_NAME);
		}

		[Test]
		[TestCase(false)]
		[Category("ImportApiIntegrationTestsForRelativityPipeline"), Category("testtype.cd")]
		public async Task ItShouldNotImportDuplicateFields(bool multiObjectField)
		{
			try
			{
				if (multiObjectField)
				{
					// Arrange
					await CreateMultiObjectFieldInstances().ConfigureAwait(false);

					// Act
					this.ImportMultiObjectFieldInstances();
				}
			}
			catch (kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException e)
			{
				// Assert
				Assert.That(e.DetailedException, Is.Not.Null);
				Assert.That(e.DetailedException.ExceptionType, Is.EqualTo("kCura.Data.RowDataGateway.ExecuteSQLStatementFailedException"));
				Assert.That(e.DetailedException.ExceptionFullText, Does.Contain("subquery returned more than 1 value").IgnoreCase);
			}
			catch (Exception e)
			{
				Assert.Fail("Importing duplicates threw a '{0}' unexpected exception type.", e.GetType());
			}
		}

		private void ConfigureJob(Settings settings, int artifactTypeId, string identifierFieldName, int identityFieldId)
		{
			settings.CaseArtifactId = WorkspaceId;
			settings.SelectedIdentifierFieldName = identifierFieldName;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.CopyFilesToDocumentRepository = false;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.StartRecordNumber = 0;
			settings.Billable = false;
			settings.LoadImportedFullTextFromServer = false;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.ArtifactTypeId = artifactTypeId;
			settings.IdentityFieldId = identityFieldId;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.MultiValueDelimiter = ';';
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
		}

		private void ImportMultiObjectFieldInstances()
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();
			ImportBulkArtifactJob importJob = importApi.NewObjectImportJob(_imagingProfileObjectTypeId);
			ConfigureJob(importJob.Settings,
				_imagingProfileObjectTypeId,
				_IMAGING_PROFILE_NAME_FIELD,
				_imagingProfileIdentityFieldId);
			ImportApiTestErrorHandler.Subscribe(importJob);
			importJob.SourceData.SourceData = CreateMultiObjectFieldDataReader();
			importJob.Execute();
		}

		private IDataReader CreateMultiObjectFieldDataReader()
		{
			var dt = new DataTable("Input Data");
			dt.Columns.Add(_IMAGING_PROFILE_NAME_FIELD);
			dt.Columns.Add(_IMAGING_PROFILE_RESTRICTED_NATIVES_FIELD);
			DataRow r = dt.NewRow();
			r[_IMAGING_PROFILE_NAME_FIELD] = _name;
			r[_IMAGING_PROFILE_RESTRICTED_NATIVES_FIELD] = $"{_associatedTypeDuplicateName}";
			dt.Rows.Add(r);
			return dt.CreateDataReader();
		}

		private async Task CreateMultiObjectFieldInstances()
		{
			await this.CreateInstanceAsync(_NATIVE_TYPE_OBJECT_TYPE_GUID, _NATIVE_TYPE_NAME_FIELD_GUID,
				_associatedTypeDuplicateName).ConfigureAwait(false);
			await this.CreateInstanceAsync(_NATIVE_TYPE_OBJECT_TYPE_GUID, _NATIVE_TYPE_NAME_FIELD_GUID,
				_associatedTypeDuplicateName).ConfigureAwait(false);
		}

		private Task CreateInstanceAsync(Guid objectTypeGuid, Guid fieldTypeGuid, string fieldValue)
		{
			var request = new CreateRequest
			{
				ObjectType = new ObjectTypeRef { Guid = objectTypeGuid },
				FieldValues = new[]
				{
					new FieldRefValuePair
					{
						Field = new FieldRef
						{
							Guid = fieldTypeGuid
						},

						Value = fieldValue
					}
				}
			};

			return this.CreateInstanceAsync(request);
		}

		private async Task CreateInstanceAsync(CreateRequest request)
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				CreateResult result = await objectManager.CreateAsync(WorkspaceId, request).ConfigureAwait(false);
				Assert.That(result.Object.ArtifactID, Is.GreaterThan(0));
			}
		}
	}
}