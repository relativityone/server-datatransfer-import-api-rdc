using System.Data;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using NUnit.Framework;

namespace kCura.Relativity.ImportAPI.IntegrationTests.AuthenticationTests
{
	[TestFixture]
	[Explicit("Relativity instance needs to be configured to work with integrated authentication")]
	public class NewImageImportJobTests : TestBase
	{
		private const string _BATES_NUMBER_COLUMN_NAME = "Bates Beg";
		private const string _IMAGE_FILE_COLUMN_NAME = "Image";

		[Test]
		public void ItShouldImportImagesWithPasswordAuthentication()
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();
			ImportImagesIntoWorkspace(importApi);
		}

		[Test]
		public void ItShouldImportImagesWithIntegratedAuthentication()
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithIntegratedAuthentication();
			ImportImagesIntoWorkspace(importApi);
		}

		private void ImportImagesIntoWorkspace(ImportAPI importApi)
		{
			ImageImportBulkArtifactJob job = importApi.NewImageImportJob();
			ImportIntoWorkspace(job, SetupImageImportJob);
		}
		private void SetupImageImportJob(ImageImportBulkArtifactJob job)
		{
			SetupJobSettings(job.Settings);
			job.SourceData.SourceData = CreateImageImportDataTable();
		}

		private DataTable CreateImageImportDataTable()
		{
			var dt = new DataTable("Image import");
			dt.Columns.Add(_CONTROL_NUMBER_COLUMN_NAME);
			
			dt.Columns.Add(_BATES_NUMBER_COLUMN_NAME);
			dt.Columns.Add(_IMAGE_FILE_COLUMN_NAME);

			DataRow r = dt.NewRow();
			r[_CONTROL_NUMBER_COLUMN_NAME] = "IMAGE001";
			r[_BATES_NUMBER_COLUMN_NAME] = "IMAGE001_1";
			r[_IMAGE_FILE_COLUMN_NAME] = @"TestData\Image\PRODSET000001.TIF";
			dt.Rows.Add(r);

			return dt;
		}

		private void SetupJobSettings(ImageSettings settings)
		{
			base.SetupJobSettings(settings);

			settings.DocumentIdentifierField = _CONTROL_NUMBER_COLUMN_NAME;
			settings.ImageFilePathSourceFieldName = _IMAGE_FILE_COLUMN_NAME;
			settings.FileLocationField = _IMAGE_FILE_COLUMN_NAME;
			settings.BatesNumberField = _BATES_NUMBER_COLUMN_NAME;

			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.ArtifactTypeId = 10;
			settings.IdentityFieldId = 1003667; // Control Number
			settings.DisableImageTypeValidation = true;
			settings.DisableImageLocationValidation = true;
		}
	}
}
