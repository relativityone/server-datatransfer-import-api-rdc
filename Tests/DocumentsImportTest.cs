using FluentAssertions;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using kCura.Relativity.DataReaderClient;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Data;
using System.IO;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using Constants = kCura.Relativity.ImportAPI.IntegrationTests.Helpers.Constants;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	public class DocumentsImportTest : TestBase
	{
		public const string NATIVE_FILE_COLUMN_NAME = "Native";

		[Test]
		[Category("ImportApiIntegrationTestsForRelativityPipeline")]
		public void ImportApiShouldImportDocuments()
		{
			// Arrange
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();

			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			ConfigureJob(job);
			DataTable importData = CreateDataTable();
			job.SourceData.SourceData = importData.CreateDataReader();
			ImportApiTestErrorHandler.Subscribe(job);

			// Act
			job.Execute();

			// Assert
			int rowsCount = importData.Rows.Count;
			GetDocumentsCount().Should().Be(rowsCount);
		}

		private int GetDocumentsCount()
		{
			using (var client = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME,
					SharedTestVariables.DEFAULT_PASSWORD))
			{
				var queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef { ArtifactTypeID = (int) ArtifactType.Document }
				};
				const int maxItemsToFetch = 10;
				QueryResult result = client.QueryAsync(WorkspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();

				return result.TotalCount;
			}
		}

		private void ConfigureJob(ImportBulkArtifactJob job)
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
			settings.NativeFilePathSourceFieldName = NATIVE_FILE_COLUMN_NAME;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.ArtifactTypeId = Constants.DOCUMENT_ARTIFACT_TYPE_ID;
			settings.IdentityFieldId = Constants.CONTROL_NUMBER_FIELD_ID;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
		}

		private DataTable CreateDataTable()
		{
			var dt = new DataTable("Input Data");
			dt.Columns.Add(DocumentIdentifierColumnName);
			dt.Columns.Add(NATIVE_FILE_COLUMN_NAME);
			dt.Columns.Add("Extracted Text");

			DataRow r = dt.NewRow();
			r[DocumentIdentifierColumnName] = "NATIVE_001";
			r[NATIVE_FILE_COLUMN_NAME] = GetNativeFilePath();
			r["Extracted Text"] = "Extracted text of 1 document.";
			dt.Rows.Add(r);

			r = dt.NewRow();
			r[DocumentIdentifierColumnName] = "NATIVE_002";
			r[NATIVE_FILE_COLUMN_NAME] = GetNativeFilePath();
			r["Extracted Text"] = "Extracted text of 2 document.";
			dt.Rows.Add(r);

			return dt;
		}

		private static string GetNativeFilePath()
		{
			string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			return Path.Combine(currentDirectory, @"TestData\Native\SBECK_0048460.docx");
		}
	}
}
