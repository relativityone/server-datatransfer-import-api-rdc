using kCura.NUnit.Integration;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using kCura.Relativity.ImportAPI.IntegrationTests.Services;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	[TestFixture]
	public class ImageProfilesImportTest : TestBase
	{
		private int _imagingProfileObjectTypeId;
		private int _imagingProfileIdentityFieldId;
		private const string _IMAGING_PROFILE_OBJECT_TYPE_NAME = "Imaging Profile";
		private const string _IMAGING_PROFILE_NAME_FIELD = "Name";
		private const string _IMAGING_PROFILE_RESTRICTED_NATIVES_FIELD = "Restricted Native Types";
		private readonly string _duplicatedName = Guid.NewGuid().ToString();
		private readonly string _uniqueNameLinked = Guid.NewGuid().ToString();
		private readonly string _uniqueNameNotLinked = Guid.NewGuid().ToString();
		private readonly string _imagingProfileName = Guid.NewGuid().ToString();
		private static readonly Guid _NATIVE_TYPE_OBJECT_TYPE_GUID = Guid.Parse("FAB01339-E3AF-4B3C-B547-2D3AF9DA5800");
		private static readonly Guid _NATIVE_TYPE_NAME_FIELD_GUID = Guid.Parse("3334FA5A-6138-4ADC-BBA1-57E3A5904623");

		[OneTimeSetUp]
		public override void OneTimeSetUp()
		{
			base.OneTimeSetUp();
			_imagingProfileIdentityFieldId = FieldService.GetIdentifierFieldId(WorkspaceId, _IMAGING_PROFILE_OBJECT_TYPE_NAME);
			_imagingProfileObjectTypeId = ObjectTypeService.GetArtifactTypeId(WorkspaceId, _IMAGING_PROFILE_OBJECT_TYPE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		[Test]
		[Category("ImportApiIntegrationTestsForRelativityPipeline"), Category("testtype.cd")]
		public async Task ItShouldLinkMultiObjectFieldsToAllMatchingObjects() // REL-158418
		{
			// Arrange
			var expectedLinkedNativesTypes = new List<int>();
			await CreateNativeTypes(expectedLinkedNativesTypes);

			// Act
			ImportImagingProfile();

			// Assert
			List<int> linked = GetLinkedRestrictedNativesTypes().ToList();

			Assert.AreEqual(expectedLinkedNativesTypes.Count, linked.Count);
			foreach (int expectedLinkedNativesType in expectedLinkedNativesTypes)
			{
				Assert.Contains(expectedLinkedNativesType, linked);
			}

			// REL-277123: Once the implementation has been reverted, consider asserting
			// that the inner exception is a SqlException whose Number equals 512.
		}

		private async Task CreateNativeTypes(List<int> expectedLinkedNativesTypes)
		{
			expectedLinkedNativesTypes.Add(await CreateNativeType(_duplicatedName));
			expectedLinkedNativesTypes.Add(await CreateNativeType(_duplicatedName));
			expectedLinkedNativesTypes.Add(await CreateNativeType(_uniqueNameLinked));
			await CreateNativeType(_uniqueNameNotLinked);
		}

		private void ImportImagingProfile()
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();
			ImportBulkArtifactJob importJob = importApi.NewObjectImportJob(_imagingProfileObjectTypeId);
			ConfigureJob(importJob);
			ImportApiTestErrorHandler.Subscribe(importJob);
			importJob.SourceData.SourceData = CreateDataReader();
			importJob.Execute();
		}

		private void ConfigureJob(ImportBulkArtifactJob job)
		{
			Settings settings = job.Settings;
			settings.CaseArtifactId = WorkspaceId;
			settings.SelectedIdentifierFieldName = _IMAGING_PROFILE_NAME_FIELD;
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
			settings.ArtifactTypeId = _imagingProfileObjectTypeId;
			settings.IdentityFieldId = _imagingProfileIdentityFieldId;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.MultiValueDelimiter = ';';
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
		}

		private IDataReader CreateDataReader()
		{

			var dt = new DataTable("Input Data");
			dt.Columns.Add(_IMAGING_PROFILE_NAME_FIELD);
			dt.Columns.Add(_IMAGING_PROFILE_RESTRICTED_NATIVES_FIELD);

			DataRow r = dt.NewRow();
			r[_IMAGING_PROFILE_NAME_FIELD] = _imagingProfileName;
			r[_IMAGING_PROFILE_RESTRICTED_NATIVES_FIELD] = $"{_uniqueNameLinked};{_duplicatedName}";
			dt.Rows.Add(r);

			return dt.CreateDataReader();
		}

		private async Task<int> CreateNativeType(string name)
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				var request = new CreateRequest
				{
					ObjectType = new ObjectTypeRef { Guid = _NATIVE_TYPE_OBJECT_TYPE_GUID },
					FieldValues = new[]
					{
						new FieldRefValuePair
						{
							Field = new FieldRef
							{
								Guid = _NATIVE_TYPE_NAME_FIELD_GUID
							},
							Value = name
						}
					}
				};
				CreateResult result = await objectManager.CreateAsync(WorkspaceId, request).ConfigureAwait(false);
				return result.Object.ArtifactID;
			}
		}

		private IEnumerable<int> GetLinkedRestrictedNativesTypes()
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				var queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef
					{
						Name = _IMAGING_PROFILE_OBJECT_TYPE_NAME,
					},
					Fields = new[]
					{
						new FieldRef
						{
							Name = _IMAGING_PROFILE_RESTRICTED_NATIVES_FIELD
						}
					},
					Condition = $"'{_IMAGING_PROFILE_NAME_FIELD}' == '{_imagingProfileName}'"
				};
				QueryResult result = objectManager.QueryAsync(WorkspaceId, queryRequest, 0, 1).GetAwaiter().GetResult();
				var objectValues = result.Objects.Single().FieldValues.Single().Value as IEnumerable<RelativityObjectValue>;
				return objectValues.Select(x => x.ArtifactID);
			}
		}
	}
}
