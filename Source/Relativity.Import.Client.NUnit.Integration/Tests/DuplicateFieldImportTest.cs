using System.Collections.Generic;
using System.Data;
using System.Linq;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using NUnit.Framework;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	[TestFixture]
	public class DuplicateFieldImportTest : TestBase
	{
		private static IEnumerable<TestCaseData> SingleObjectFieldTestCases
		{
			get
			{
				yield return new TestCaseData("Transfer-1", "Transfer-Description-1", "TransferDetail-1", "TransferDataSourceName-1");
			}
		}

		private static IEnumerable<TestCaseData> MultipleObjectFieldTestCases
		{
			get
			{
				yield return new TestCaseData("Transfer-2", "Transfer-Description-2", "TransferDetail-2", "TransferDataSourceName-2");
			}
		}

		[Test]
		[TestCaseSource(nameof(MultipleObjectFieldTestCases))]
		[Category("ImportApiIntegrationTestsForRelativityPipeline"), Category("testtype.cd")]
		public void ItShouldNotImportDuplicateMultipleObjectFields(string name, string description, string detailName, string dataSourceName)
		{
			using (DataTable table = new DataTable("Input Data"))
			{
				// Arrange
				this.CreateSingleObjectInstance(detailName);
				this.CreateMultipleObjectInstance(dataSourceName);
				this.CreateMultipleObjectInstance(dataSourceName);
				this.SetupDataTable(table);
				table.Rows.Add(name, description, dataSourceName, detailName);

				// Act
				JobReport report = this.ImportObjects(table);

				// Assert
				// Note: duplicate multiple-object fields yield a fatal exception.
				Assert.That(report, Is.Not.Null);
				Assert.That(report.ErrorRows.Count, Is.Zero);
				Assert.That(report.TotalRows, Is.EqualTo(1));
				Assert.That(report.FatalException, Is.Not.Null);
				kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException exception =
					report.FatalException as kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException;
				Assert.That(exception, Is.Not.Null);
				Assert.That(exception.DetailedException.ExceptionType, Is.EqualTo("kCura.Data.RowDataGateway.ExecuteSQLStatementFailedException"));
				Assert.That(exception.DetailedException.ExceptionFullText, Does.Contain("subquery returned more than 1 value").IgnoreCase);
				Assert.That(report.EndTime, Is.GreaterThan(report.StartTime));
				Assert.That(report.FileBytes, Is.Zero);
				Assert.That(report.MetadataBytes, Is.Zero);
			}
		}

		[Test]
		[TestCaseSource(nameof(SingleObjectFieldTestCases))]
		[Category("ImportApiIntegrationTestsForRelativityPipeline"), Category("testtype.cd")]
		public void ItShouldNotImportDuplicateSingleObjectFields(string name, string description, string detailName, string dataSourceName)
		{
			using (DataTable table = new DataTable("Input Data"))
			{
				// Arrange
				this.CreateSingleObjectInstance(detailName);
				this.CreateSingleObjectInstance(detailName);
				this.CreateMultipleObjectInstance(dataSourceName);				
				this.SetupDataTable(table);
				table.Rows.Add(name, description, dataSourceName, detailName);

				// Act
				JobReport report = this.ImportObjects(table);

				// Assert
				// Note: duplicate single-object fields yield a job-level error.
				Assert.That(report.TotalRows, Is.EqualTo(1));
				Assert.That(report.FatalException, Is.Null);
				Assert.That(report.ErrorRows.Count, Is.EqualTo(1));
				Assert.That(report.ErrorRows.First().Message,
					Does.Contain("A non unique associated object is specified for this new object").IgnoreCase);
				Assert.That(report.EndTime, Is.GreaterThan(report.StartTime));
				Assert.That(report.FileBytes, Is.Zero);
				Assert.That(report.MetadataBytes, Is.Positive);
			}
		}

		protected override void OnOneTimeSetUp()
		{
			base.OnOneTimeSetUp();
			this.CreateTransferDetailObjectType();
			this.CreateTransferDataSourceObjectType();
			this.CreateTransferObjectType();
		}

		private void ConfigureJobSettings(Settings settings)
		{
			this.ConfigureJobSettings(settings,
				this.TransferArtifactTypeId,
				this.TransferIdentifierFieldId,
				TRANSFER_FIELD_NAME);
		}

		private void ConfigureJobSettings(
			Settings settings,
			int artifactTypeId,
			int identityFieldId,
			string selectedIdentifierFieldName)
		{
			settings.CaseArtifactId = WorkspaceId;
			settings.SelectedIdentifierFieldName = selectedIdentifierFieldName;
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

		private void SetupDataTable(DataTable table)
		{
			table.Columns.Add(TRANSFER_FIELD_NAME);
			table.Columns.Add(TRANSFER_FIELD_DESCRIPTION);
			table.Columns.Add(TRANSFER_FIELD_DATASOURCEID);
			table.Columns.Add(TRANSFER_FIELD_DETAIL_ID);
		}

		private JobReport ImportObjects(DataTable table)
		{
			ImportAPI importApi = ImportApiCreator.CreateImportApiWithPasswordAuthentication();
			ImportBulkArtifactJob importJob = importApi.NewObjectImportJob(this.TransferArtifactTypeId);
			this.ConfigureJobSettings(importJob.Settings);
			using (importJob.SourceData.SourceData = table.CreateDataReader())
			{
				JobReport jobReport = null;
				importJob.OnComplete += report =>
				{
					jobReport = report;
				};

				importJob.Execute();
				return jobReport;
			}
		}
	}
}