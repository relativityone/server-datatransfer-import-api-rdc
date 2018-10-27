using System;
using System.Data;
using System.Text;
using kCura.EDDS.WebAPI.BulkImportManagerBase;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using kCura.Relativity.ImportAPI.IntegrationTests.Services;
using NUnit.Framework;
using Constants = kCura.Relativity.ImportAPI.IntegrationTests.Helpers.Constants;
using Settings = kCura.Relativity.DataReaderClient.Settings;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	[TestFixture]
	[Category("ImportApiIntegrationTestsForRelativityPipeline"), Category("testtype.cd")]
	public class ProductionImportTests : TestBase
	{
		private const string _BATES_NUMBER_COLUMN_NAME = "Bates Beg";
		private const string _IMAGE_FILE_COLUMN_NAME = "Image";
		private const string _IMAGE_FILE_PATH = @"TestData\Image\PRODSET000001.TIF";
		private const string _FIRST_DOCUMENT_CONTROL_NUMBER = "PRODSET000001";
		private const string _SECOND_DOCUMENT_CONTROL_NUMBER = "PRODSET001502";

		[Test]
		public void ItShouldSetCorrectBatesNumbersWhenImportingMoreThanThousandImages()
		{
			// Arrange
			ImportDocuments();
			int productionId = ProductionService.Create(WorkspaceId, "production");

			// Act
			ImportProduction(productionId);

			// Assert
			Tuple<string, string> batesNumbers = ProductionService.GetProductionBatesNumbers(WorkspaceId, productionId);
			string expectedFirstBatesValue = _FIRST_DOCUMENT_CONTROL_NUMBER;
			string expectedLastBatesValue = _SECOND_DOCUMENT_CONTROL_NUMBER;
			Assert.AreEqual(expectedFirstBatesValue, batesNumbers.Item1);
			Assert.AreEqual(expectedLastBatesValue, batesNumbers.Item2);
		}

		private void ImportDocuments()
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();
			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			ImportApiTestErrorHandler.Subscribe(job);
			ConfigureDocumentImportJob(job);
			DataTable importData = CreateDataTable();
			job.SourceData.SourceData = importData.CreateDataReader();
			job.Execute();
		}

		private void ImportProduction(int productionId)
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();
			ImageImportBulkArtifactJob productionImportJob = importApi.NewProductionImportJob(productionId);
			ImportApiTestErrorHandler.Subscribe(productionImportJob);
			ConfigureImageImportJob(productionImportJob);
			productionImportJob.SourceData.SourceData = CreateImageImportDataTable();
			productionImportJob.Execute();
		}

		private void ConfigureDocumentImportJob(ImportBulkArtifactJob job)
		{
			Settings settings = job.Settings;
			settings.CaseArtifactId = WorkspaceId;
			settings.SelectedIdentifierFieldName = DocumentIdentifierColumnName;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.StartRecordNumber = 0;
			settings.Billable = false;
			settings.LoadImportedFullTextFromServer = false;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.ArtifactTypeId = Constants.DOCUMENT_ARTIFACT_TYPE_ID;
			settings.IdentityFieldId = Constants.CONTROL_NUMBER_FIELD_ID;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = true;
			settings.DisableNativeValidation = true;
		}

		public void ConfigureImageImportJob(ImageImportBulkArtifactJob job)
		{
			ImageSettings settings = job.Settings;
			settings.AuditLevel = ImportAuditLevel.NoSnapshot;
			settings.CaseArtifactId = WorkspaceId;
			settings.CopyFilesToDocumentRepository = false;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextFieldContainsFilePath = true;
			settings.SelectedIdentifierFieldName = DocumentIdentifierColumnName;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.OverwriteMode = OverwriteModeEnum.Overlay;
			settings.OverlayBehavior = OverlayBehavior.MergeAll;
			settings.Billable = false;
			settings.AutoNumberImages = false;
			settings.ForProduction = true;
			settings.ExtractedTextEncoding = Encoding.UTF8;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.DocumentIdentifierField = DocumentIdentifierColumnName;
			settings.ImageFilePathSourceFieldName = _IMAGE_FILE_COLUMN_NAME;
			settings.FileLocationField = _IMAGE_FILE_COLUMN_NAME;
			settings.BatesNumberField = _BATES_NUMBER_COLUMN_NAME;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.ArtifactTypeId = Constants.DOCUMENT_ARTIFACT_TYPE_ID;
			settings.IdentityFieldId = Constants.CONTROL_NUMBER_FIELD_ID;
			settings.DisableImageTypeValidation = true;
			settings.DisableImageLocationValidation = true;
		}

		private DataTable CreateDataTable()
		{
			var dt = new DataTable("Input Data");
			dt.Columns.Add(DocumentIdentifierColumnName);

			DataRow r = dt.NewRow();
			r[DocumentIdentifierColumnName] = _FIRST_DOCUMENT_CONTROL_NUMBER;
			dt.Rows.Add(r);

			r = dt.NewRow();
			r[DocumentIdentifierColumnName] = _SECOND_DOCUMENT_CONTROL_NUMBER;
			dt.Rows.Add(r);

			return dt;
		}

		private DataTable CreateImageImportDataTable()
		{
			var dt = new DataTable("Image import");
			dt.Columns.Add(DocumentIdentifierColumnName);

			dt.Columns.Add(_BATES_NUMBER_COLUMN_NAME);
			dt.Columns.Add(_IMAGE_FILE_COLUMN_NAME);
			DataRow row;
			const int numberOfImagesForFirstDoc = 1501;
			for (int i = 1; i <= numberOfImagesForFirstDoc; i++)
			{
				row = dt.NewRow();
				row[DocumentIdentifierColumnName] = _FIRST_DOCUMENT_CONTROL_NUMBER;
				row[_BATES_NUMBER_COLUMN_NAME] = $"PRODSET{i:D6}";
				row[_IMAGE_FILE_COLUMN_NAME] = _IMAGE_FILE_PATH;
				dt.Rows.Add(row);
			}

			row = dt.NewRow();
			row[DocumentIdentifierColumnName] = _SECOND_DOCUMENT_CONTROL_NUMBER;
			row[_BATES_NUMBER_COLUMN_NAME] = _SECOND_DOCUMENT_CONTROL_NUMBER;
			row[_IMAGE_FILE_COLUMN_NAME] = _IMAGE_FILE_PATH;
			dt.Rows.Add(row);

			return dt;
		}
	}
}
