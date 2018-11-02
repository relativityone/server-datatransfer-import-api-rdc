﻿using System;
using System.Data;
using System.IO;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using NUnit.Framework;

namespace kCura.Relativity.ImportAPI.IntegrationTests.AuthenticationTests
{
	[TestFixture]
	public class NewNativeDocumentImportJobAuthenticationTests : AuthenticationTestBase
	{
		private const string _NATIVE_FILE_COLUMN_NAME = "Native";

		[Test]
		[Category("ImportApiIntegrationTestsForRelativityPipeline"), Category("testtype.cd")]
		public void ItShouldImportNativesWithPasswordAuthentication()
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();
			ImportDocumentsIntoWorkspace(importApi);
		}

		[Test]
		[Explicit("Relativity instance needs to be configured to work with integrated authentication. Please see readme.txt")]
		public void ItShouldImportNativesWithIntegratedAuthentication()
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithIntegratedAuthentication();
			ImportDocumentsIntoWorkspace(importApi);
		}
		
		private void ImportDocumentsIntoWorkspace(ImportAPI importApi)
		{
			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			ImportIntoWorkspace(job, SetupNativeImportJob);
		}

		private void SetupNativeImportJob(ImportBulkArtifactJob job)
		{
			SetupJobSettings(job.Settings);
			job.SourceData.SourceData = CreateDataReader();
		}

		private IDataReader CreateDataReader()
		{
			var dt = new DataTable("Input Data");
			dt.Columns.Add(DocumentIdentifierColumnName);
			dt.Columns.Add(_NATIVE_FILE_COLUMN_NAME);

			DataRow r = dt.NewRow();
			r[DocumentIdentifierColumnName] = "NATIVE_001";
			string nativeLocation = GetNativeFilePath();

			r[_NATIVE_FILE_COLUMN_NAME] = nativeLocation;
			dt.Rows.Add(r);

			return dt.CreateDataReader();
		}

		private static string GetNativeFilePath()
		{
			string currentDirectory = TestContext.CurrentContext.TestDirectory;
			return Path.Combine(currentDirectory, @"TestData\Native\SBECK_0048460.docx");
		}

		private void SetupJobSettings(Settings settings)
		{
			base.SetupJobSettings(settings);

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
	}
}
