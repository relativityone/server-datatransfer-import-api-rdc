using FluentAssertions;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using kCura.Relativity.DataReaderClient;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Platform.Keywords.RSAPI;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Constants = kCura.Relativity.ImportAPI.IntegrationTests.Helpers.Constants;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	public class DocumentsImportTest
	{
		const string _CONTROL_NUMBER_COLUMN_NAME = "Control Number";

		const string _NATIVE_FILE_COLUMN_NAME = "Native";

		private int? _workspaceId;

		[SetUp]
		public void CreateWorkspace()
		{
			using (IRSAPIClient rsapiClient = ServiceFactory.GetProxy<IRSAPIClient>(SharedTestVariables.ADMIN_USERNAME, 
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				string now = DateTime.Now.ToString("MM-dd HH.mm.ss.fff");
				_workspaceId =
					WorkspaceHelpers.CreateWorkspace(rsapiClient, $"Import API test workspace ({now})", "Relativity Starter Template");
				WorkspaceHelpers.MarkTestWorkspaceAsUsed(_workspaceId.Value);
			}
		}

		[TearDown]
		public void DeleteWorkspace()
		{
			if (_workspaceId.HasValue)
			{
				WorkspaceHelpers.DeleteTestWorkspace(_workspaceId.Value);
			}
		}

		[Test]
		[Category("ImportApiIntegrationTestsForRelativityPipeline")]
		public void ImportApiShouldImportDocuments()
		{
			// Arrange
			string url = SharedTestVariables.SERVER_BINDING_TYPE + "://" + SharedTestVariables.RSAPI_SERVER_ADDRESS + "/RelativityWebApi";
			var importApi = new ImportAPI(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD, url);

			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			ConfigureJob(job);
			job.SourceData.SourceData = CreateDataReader();
			job.OnComplete += JobOnOnComplete;
			job.OnFatalException += JobOnOnFatalException;

			// Act
			job.Execute();

			// Assert
			GetDocumentsCount().Should().Be(2);
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
				QueryResult result = client.QueryAsync(_workspaceId.Value, queryRequest, 1, 100).GetAwaiter().GetResult();

				return result.Objects.Count;
			}
		}

		private void ConfigureJob(ImportBulkArtifactJob job)
		{
			Settings settings = job.Settings;
			settings.CaseArtifactId = _workspaceId.Value;
			settings.SelectedIdentifierFieldName = _CONTROL_NUMBER_COLUMN_NAME;
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
			settings.NativeFilePathSourceFieldName = _NATIVE_FILE_COLUMN_NAME;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.ArtifactTypeId = Constants.DOCUMENT_ARTIFACT_TYPE_ID;
			settings.IdentityFieldId = Constants.CONTROL_NUMBER_FIELD_ID;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = true;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
		}

		private void JobOnOnComplete(JobReport jobreport)
		{
			if (jobreport.FatalException != null)
			{
				throw jobreport.FatalException;
			}

			if (jobreport.ErrorRowCount > 0)
			{
				IEnumerable<string> errors = jobreport.ErrorRows.Select(x => $"{x.Identifier} - {x.Message}");
				throw new Exception(string.Join("\n", errors));
			}
		}

		private void JobOnOnFatalException(JobReport jobreport)
		{
			throw jobreport.FatalException;
		}

		private IDataReader CreateDataReader()
		{
			var dt = new DataTable("Input Data");
			dt.Columns.Add(_CONTROL_NUMBER_COLUMN_NAME);
			dt.Columns.Add(_NATIVE_FILE_COLUMN_NAME);
			dt.Columns.Add("Extracted Text");

			DataRow r = dt.NewRow();
			r[_CONTROL_NUMBER_COLUMN_NAME] = "NATIVE_001";
			r[_NATIVE_FILE_COLUMN_NAME] = GetNativeFilePath();
			r["Extracted Text"] = "Extracted text of 1 document.";
			dt.Rows.Add(r);

			r = dt.NewRow();
			r[_CONTROL_NUMBER_COLUMN_NAME] = "NATIVE_002";
			r[_NATIVE_FILE_COLUMN_NAME] = GetNativeFilePath();
			r["Extracted Text"] = "Extracted text of 2 document.";
			dt.Rows.Add(r);

			return dt.CreateDataReader();
		}

		private static string GetNativeFilePath()
		{
			string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			return Path.Combine(currentDirectory, @"TestData\Native\SBECK_0048460.docx");
		}
	}
}
