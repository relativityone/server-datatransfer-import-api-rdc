// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportJobTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents an abstract load-file base class.
	/// </summary>
	public abstract class ImportJobTestBase : TestBase
	{
		/// <summary>
		/// The minimum test file length [1KB].
		/// </summary>
		internal const int MinTestFileLength = 1024;

		/// <summary>
		/// The maximum test file length [10KB].
		/// </summary>
		internal const int MaxTestFileLength = 10 * MinTestFileLength;

		/// <summary>
		/// The thread synchronization backing.
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// The job messages.
		/// </summary>
		private readonly System.Collections.Generic.List<string> jobMessages = new global::System.Collections.Generic.List<string>();

		/// <summary>
		/// The job fatal exceptions.
		/// </summary>
		private readonly System.Collections.Generic.List<Exception> jobFatalExceptions = new global::System.Collections.Generic.List<Exception>();

		/// <summary>
		/// The error rows.
		/// </summary>
		private readonly System.Collections.Generic.List<IDictionary> errorRows = new global::System.Collections.Generic.List<IDictionary>();

		/// <summary>
		/// The progress completed rows.
		/// </summary>
		private readonly System.Collections.Generic.List<long> progressCompletedRows = new global::System.Collections.Generic.List<long>();

		/// <summary>
		/// The import job.
		/// </summary>
		private ImportBulkArtifactJob importJob;

		/// <summary>
		/// The completed job report.
		/// </summary>
		private JobReport completedJobReport;

		/// <summary>
		/// Gets or sets source data.
		/// </summary>
		/// <value>
		/// The <see cref="DataTable"/> instance.
		/// </value>
		protected DataTable SourceData
		{
			get;
			set;
		}

		/// <summary>
		/// The test setup.
		/// </summary>
		protected override void OnSetup()
		{
			base.OnSetup();
			this.SourceData = new DataTable { Locale = CultureInfo.InvariantCulture };
			this.SourceData.Columns.Add(WellKnownFields.ControlNumber, typeof(string));
			this.SourceData.Columns.Add(WellKnownFields.FilePath, typeof(string));
			this.jobMessages.Clear();
			this.jobFatalExceptions.Clear();
			this.errorRows.Clear();
			this.progressCompletedRows.Clear();
			this.importJob = null;
			this.completedJobReport = null;
		}

		/// <summary>
		/// The test tear down.
		/// </summary>
		protected override void OnTearDown()
		{
			this.SourceData?.Dispose();
			if (this.importJob != null)
			{
				this.importJob.OnError -= this.ImportJob_OnError;
				this.importJob.OnFatalException -= this.ImportJob_OnFatalException;
				this.importJob.OnMessage -= this.ImportJob_OnMessage;
				this.importJob.OnComplete -= this.ImportJob_OnComplete;
				this.importJob.OnProgress -= this.ImportJob_OnProgress;
			}

			base.OnTearDown();
		}

		/// <summary>
		/// Given the dataset path to import.
		/// </summary>
		/// <param name="file">
		/// The file to import.
		/// </param>
		protected void GivenTheDatasetPathToImport(string file)
		{
			var controlId = Path.GetFileName(file) + " - " + this.Timestamp.Ticks;
			this.SourceData.Rows.Add(controlId + " " + 1, file);
		}

		/// <summary>
		/// Given the source files are locked.
		/// </summary>
		/// <param name="index">
		/// Specify the zero-based index.
		/// </param>
		protected void GivenTheSourceFileIsLocked(int index)
		{
			var filePath = this.SourceData.Rows[index][1].ToString();
			ChangeFileFullPermissions(filePath, false);
		}

		/// <summary>
		/// Given the import job.
		/// </summary>
		protected void GivenTheImportJob()
		{
			var iapi = new ImportAPI(
				this.TestParameters.RelativityUserName,
				this.TestParameters.RelativityPassword,
				this.TestParameters.RelativityWebApiUrl.ToString());
			this.importJob = iapi.NewNativeDocumentImportJob();
			this.importJob.Settings.WebServiceURL = this.TestParameters.RelativityWebApiUrl.ToString();
			this.importJob.Settings.CaseArtifactId = this.TestParameters.WorkspaceId;
			this.importJob.Settings.ArtifactTypeId = 10;
			this.importJob.Settings.ExtractedTextFieldContainsFilePath = false;
			this.importJob.Settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			this.importJob.Settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			this.importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			this.importJob.Settings.OverwriteMode = OverwriteModeEnum.Append;
			this.importJob.Settings.OIFileIdMapped = true;
			this.importJob.Settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			this.importJob.Settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
			this.importJob.Settings.ExtractedTextEncoding = Encoding.Unicode;
			this.importJob.Settings.FileSizeMapped = true;
			this.importJob.Settings.FileSizeColumn = WellKnownFields.NativeFileSize;
			this.importJob.SourceData.SourceData = this.SourceData.CreateDataReader();
			this.importJob.OnError += this.ImportJob_OnError;
			this.importJob.OnFatalException += this.ImportJob_OnFatalException;
			this.importJob.OnMessage += this.ImportJob_OnMessage;
			this.importJob.OnComplete += this.ImportJob_OnComplete;
			this.importJob.OnProgress += this.ImportJob_OnProgress;
		}

		/// <summary>
		/// When executing the import job.
		/// </summary>
		protected void WhenExecutingTheJob()
		{
			var sw = Stopwatch.StartNew();
			this.importJob.Execute();
			sw.Stop();
			Console.WriteLine("Import API elapsed time: {0}", sw.Elapsed);
		}

		/// <summary>
		/// Then the import job is successful.
		/// </summary>
		protected void ThenTheImportJobIsSuccessful()
		{
			Assert.That(this.errorRows.Count, Is.EqualTo(0));
			Assert.That(this.jobFatalExceptions.Count, Is.EqualTo(0));
			Assert.That(this.completedJobReport, Is.Not.Null);
			Assert.That(this.completedJobReport.ErrorRows.Count, Is.EqualTo(0));
			Assert.That(this.completedJobReport.FatalException, Is.Null);
			Assert.That(this.completedJobReport.TotalRows, Is.EqualTo(this.SourceData.Rows.Count));
		}

		/// <summary>
		/// Then the import job is successful.
		/// </summary>
		/// <param name="expectedErrorRows">
		/// The expected number of error rows.
		/// </param>
		/// <param name="expectedTotalRows">
		/// The expected number of total rows.
		/// </param>
		/// <param name="fatalExceptions">
		/// Specify whether fatal exceptions are expected.
		/// </param>
		protected void ThenTheImportJobIsNotSuccessful(int expectedErrorRows, int expectedTotalRows, bool fatalExceptions)
		{
			Assert.That(this.errorRows.Count, Is.EqualTo(expectedErrorRows));
			if (fatalExceptions)
			{
				Assert.That(this.jobFatalExceptions.Count, Is.GreaterThan(0));
				Assert.That(this.completedJobReport.FatalException, Is.Not.Null);

				// Note: the exact number of expected rows can vary over a range when expecting an error.
				Assert.That(this.completedJobReport.TotalRows, Is.AtLeast(1).And.LessThanOrEqualTo(expectedTotalRows));
			}
			else
			{
				Assert.That(this.jobFatalExceptions.Count, Is.EqualTo(0));
				Assert.That(this.completedJobReport.FatalException, Is.Null);
				Assert.That(this.completedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));
			}

			Assert.That(this.completedJobReport, Is.Not.Null);
			Assert.That(this.completedJobReport.ErrorRows.Count, Is.EqualTo(expectedErrorRows));
		}

		/// <summary>
		/// Then the import progress events are raised.
		/// </summary>
		protected void ThenTheImportProgressEventsAreRaised()
		{
			this.ThenTheImportProgressEventsCountShouldEqual(this.SourceData.Rows.Count);
		}

		/// <summary>
		/// Then the import progress events count should equal the specified value.
		/// </summary>
		/// <param name="expected">
		/// The expected count.
		/// </param>
		protected void ThenTheImportProgressEventsCountShouldEqual(int expected)
		{
			Assert.That(this.progressCompletedRows.Count, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the import progress events count should be greater than zero.
		/// </summary>
		protected void ThenTheImportProgressEventsCountIsNonZero()
		{
			Assert.That(this.progressCompletedRows.Count, Is.GreaterThan(0));
		}

		/// <summary>
		/// Then the import message count is non zero.
		/// </summary>
		protected void ThenTheImportMessageCountIsNonZero()
		{
			Assert.That(this.jobMessages.Count, Is.GreaterThan(0));
		}

		/// <summary>
		/// Then the import messages contains the specified message.
		/// </summary>
		/// <param name="message">
		/// The message to check.
		/// </param>
		protected void ThenTheImportMessagesContains(string message)
		{
			Assert.That(this.jobMessages.Any(x => x.Contains(message)), Is.True);
		}

		/// <summary>
		/// Creates the unique control identifier.
		/// </summary>
		/// <param name="file">
		/// The full path to the source file.
		/// </param>
		/// <returns>
		/// The control identifier.
		/// </returns>
		protected string CreateUniqueControlId(string file)
		{
			return Path.GetFileName(file) + " - " + this.Timestamp.Ticks;
		}

		/// <summary>
		/// Given the dataset is auto-generated by the number of specified files.
		/// </summary>
		/// <param name="maxFiles">
		/// The file limit.
		/// </param>
		/// <param name="includeReadOnlyFiles">
		/// Specify whether to include read-only files in the dataset.
		/// </param>
		protected void GivenTheAutoGeneratedDatasetToImport(int maxFiles, bool includeReadOnlyFiles)
		{
			for (var i = 0; i < maxFiles; i++)
			{
				RandomHelper.NextTextFile(
					MinTestFileLength,
					MaxTestFileLength,
					this.TempDirectory.Directory,
					includeReadOnlyFiles && i % 2 == 0);
			}

			this.GivenTheDatasetPathToImport(this.TempDirectory.Directory, "*", SearchOption.AllDirectories);
		}

		/// <summary>
		/// Given the dataset path to import.
		/// </summary>
		/// <param name="path">
		/// The dataset path to import.
		/// </param>
		/// <param name="searchPattern">
		/// Specify the search pattern.
		/// </param>
		/// <param name="searchOption">
		/// Specify the search option.
		/// </param>
		protected void GivenTheDatasetPathToImport(string path, string searchPattern, SearchOption searchOption)
		{
			var number = 1;
			foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
			{
				var controlId = this.CreateUniqueControlId(file);
				this.SourceData.Rows.Add(controlId + " " + number, file);
			}
		}

		/// <summary>
		/// The import job complete handler.
		/// </summary>
		/// <param name="jobReport">
		/// The job report.
		/// </param>
		private void ImportJob_OnComplete(JobReport jobReport)
		{
			lock (SyncRoot)
			{
				this.completedJobReport = jobReport;
			}
		}

		/// <summary>
		/// The import job progress handler.
		/// </summary>
		/// <param name="completedRow">
		/// The completed row.
		/// </param>
		private void ImportJob_OnProgress(long completedRow)
		{
			lock (SyncRoot)
			{
				this.progressCompletedRows.Add(completedRow);
			}
		}

		/// <summary>
		/// The import job message handler.
		/// </summary>
		/// <param name="status">
		/// The status.
		/// </param>
		private void ImportJob_OnMessage(Status status)
		{
			lock (SyncRoot)
			{
				this.jobMessages.Add(status.Message);
				Console.WriteLine(status.Message);
			}
		}

		/// <summary>
		/// The import job fatal exception handler.
		/// </summary>
		/// <param name="jobReport">
		/// The job report.
		/// </param>
		private void ImportJob_OnFatalException(JobReport jobReport)
		{
			lock (SyncRoot)
			{
				this.jobFatalExceptions.Add(jobReport.FatalException);
				Console.WriteLine(jobReport.FatalException.ToString());
			}
		}

		/// <summary>
		/// The import job error handler.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		private void ImportJob_OnError(IDictionary row)
		{
			lock (SyncRoot)
			{
				this.errorRows.Add(row);
			}
		}
	}
}