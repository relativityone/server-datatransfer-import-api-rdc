// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportJobTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.IO;
	using System.Net;

	using global::NUnit.Framework;

	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	public abstract class ImportJobTestBase : IDisposable
	{
		protected ImportJobTestBase()
		{
			Assume.That(AssemblySetup.TestParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
		}

		protected ImportTestJobResult TestJobResult { get; private set; }

		protected ImportAPI ImportAPI { get; set; }

		protected TempDirectory2 TempDirectory { get; private set; }

		protected DateTime Timestamp { get; private set; }

		[SetUp]
		public void Setup()
		{
			this.Timestamp = DateTime.Now;
			this.TempDirectory = new TempDirectory2();
			this.TempDirectory.Create();
			this.TestJobResult = new ImportTestJobResult();

			AppSettings.Instance.IoErrorWaitTimeInSeconds = 0;
			AppSettings.Instance.IoErrorNumberOfRetries = 0;

			kCura.WinEDDS.Config.ConfigSettings["BadPathErrorsRetry"] = false;
			kCura.WinEDDS.Config.ConfigSettings["ForceWebUpload"] = false;
			kCura.WinEDDS.Config.ConfigSettings["PermissionErrorsRetry"] = false;
			kCura.WinEDDS.Config.ConfigSettings["TapiMaxJobRetryAttempts"] = 1;
			kCura.WinEDDS.Config.ConfigSettings["TapiMaxJobParallelism"] = 1;
			kCura.WinEDDS.Config.ConfigSettings["TapiLogEnabled"] = true;
			kCura.WinEDDS.Config.ConfigSettings["TapiSubmitApmMetrics"] = false;

			kCura.WinEDDS.Config.ConfigSettings["UsePipeliningForFileIdAndCopy"] = false;
			kCura.WinEDDS.Config.ConfigSettings["TapiForceAsperaClient"] = false;
			kCura.WinEDDS.Config.ConfigSettings["TapiForceFileShareClient"] = false;
			kCura.WinEDDS.Config.ConfigSettings["TapiForceHttpClient"] = false;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = false;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = false;

			// Note: there's no longer a BCP sub-folder.
			kCura.WinEDDS.Config.ConfigSettings["TapiAsperaBcpRootFolder"] = string.Empty;
			kCura.WinEDDS.Config.ConfigSettings["TapiAsperaNativeDocRootLevels"] = 1;
		}

		[TearDown]
		public void Teardown()
		{
			if (this.TempDirectory != null)
			{
				this.TempDirectory.ClearReadOnlyAttributes = true;
				this.TempDirectory.Dispose();
				this.TempDirectory = null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected static void ForceClient(TapiClient tapiClient)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiForceAsperaClient"] = tapiClient == TapiClient.Aspera;
			kCura.WinEDDS.Config.ConfigSettings["TapiForceFileShareClient"] = tapiClient == TapiClient.Direct;
			kCura.WinEDDS.Config.ConfigSettings["TapiForceHttpClient"] = tapiClient == TapiClient.Web;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Teardown();
			}
		}

		protected void GivenTheImportJob()
		{
			this.ImportAPI = new ImportAPI(
				AssemblySetup.TestParameters.RelativityUserName,
				AssemblySetup.TestParameters.RelativityPassword,
				AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
		}

		protected void GivenTheImportJobWithIntegratedAuthentication()
		{
			this.ImportAPI = new ImportAPI(AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
		}

		protected void GivenTheDatasetPathToImport(DataTable table, string file)
		{
			string uniqueControlId = $"{Path.GetFileName(file)} - {this.Timestamp.Ticks}";
			table.Rows.Add(uniqueControlId, file);
		}

		protected void ThenTheImportJobIsSuccessful(int expectedTotalRows)
		{
			Assert.That(this.TestJobResult.ErrorRows, Has.Count.Zero);
			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Zero);
			Assert.That(this.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.Zero);
			Assert.That(this.TestJobResult.CompletedJobReport.FatalException, Is.Null);
			Assert.That(this.TestJobResult.CompletedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));
		}

		protected void ThenTheImportJobFailedWithFatalError(int expectedErrorRows, int expectedTotalRows)
		{
			// Note: the exact number of expected rows can vary over a range when expecting an error.
			Assert.That(this.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.TestJobResult.CompletedJobReport.TotalRows, Is.Positive.And.LessThanOrEqualTo(expectedTotalRows));

			Assert.That(this.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
			Assert.That(this.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(this.TestJobResult.ErrorRows.Count));

			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Positive);
			Assert.That(this.TestJobResult.CompletedJobReport.FatalException, Is.Not.Null);
		}

		protected void ThenTheImportJobCompletedWithErrors(int expectedErrorRows, int expectedTotalRows)
		{
			Assert.That(this.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.TestJobResult.CompletedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));

			Assert.That(this.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
			Assert.That(this.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(this.TestJobResult.ErrorRows.Count));

			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Zero);
			Assert.That(this.TestJobResult.CompletedJobReport.FatalException, Is.Null);
		}

		protected List<string> GetRandomTextFiles(int maxFiles, bool includeReadOnlyFiles)
		{
			var files = new List<string>();
			const int MinTestFileLength = 1024;
			const int MaxTestFileLength = 10 * MinTestFileLength;
			for (var i = 0; i < maxFiles; i++)
			{
				string file = RandomHelper.NextTextFile(
					MinTestFileLength,
					MaxTestFileLength,
					this.TempDirectory.Directory,
					includeReadOnlyFiles && i % 2 == 0);

				files.Add(file);
			}

			return files;
		}
	}
}