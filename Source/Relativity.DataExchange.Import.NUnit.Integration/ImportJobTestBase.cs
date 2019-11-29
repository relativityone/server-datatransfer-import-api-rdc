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
	using System.Linq;
	using System.Net;

	using global::NUnit.Framework;

	using kCura.Relativity.ImportAPI;

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

		protected Dictionary<string, ImportAPI> ImportAPIInstancesDict { get; } = new Dictionary<string, ImportAPI>();

		protected TempDirectory2 TempDirectory { get; private set; }

		[SetUp]
		public void Setup()
		{
			kCura.WinEDDS.Config.ConfigSettings["BadPathErrorsRetry"] = false;
			kCura.WinEDDS.Config.ConfigSettings["TapiMaxJobRetryAttempts"] = 1;
			AppSettings.Instance.TapiMaxJobParallelism = 1;
			kCura.WinEDDS.Config.ConfigSettings["TapiLogEnabled"] = true;
			AppSettings.Instance.TapiSubmitApmMetrics = false;
			kCura.WinEDDS.Config.ConfigSettings["UsePipeliningForFileIdAndCopy"] = false;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = false;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = false;
			AppSettings.Instance.IoErrorWaitTimeInSeconds = 0;
			AppSettings.Instance.IoErrorNumberOfRetries = 0;

			this.TempDirectory = new TempDirectory2();
			this.TempDirectory.Create();
			this.TestJobResult = new ImportTestJobResult();
		}

		[TearDown]
		public void Teardown()
		{
			this.ImportAPIInstancesDict.Clear();
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
			AppSettings.Instance.TapiForceAsperaClient = tapiClient == TapiClient.Aspera;
			AppSettings.Instance.TapiForceFileShareClient = tapiClient == TapiClient.Direct;
			AppSettings.Instance.TapiForceHttpClient = tapiClient == TapiClient.Web;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Teardown();
			}
		}

		protected void GivenTheImportJobs(int instanceCount)
		{
			for (int index = 0; index < instanceCount; ++index)
			{
				this.ImportAPIInstancesDict.Add($"Client-{index}", new ImportAPI(
					AssemblySetup.TestParameters.RelativityUserName,
					AssemblySetup.TestParameters.RelativityPassword,
					AssemblySetup.TestParameters.RelativityWebApiUrl.ToString()));
			}
		}

		protected void GivenTheImportJob()
		{
			this.GivenTheImportJobs(1);
		}

		protected void GivenTheImportJobsWithIntegratedAuthentication(int instanceCount)
		{
			for (int index = 0; index < instanceCount; ++index)
			{
				this.ImportAPIInstancesDict.Add($"Client-{index}", new ImportAPI(
					AssemblySetup.TestParameters.RelativityWebApiUrl.ToString()));
			}
		}

		protected void GivenTheImportJobWithIntegratedAuthentication()
		{
			this.GivenTheImportJobsWithIntegratedAuthentication(1);
		}

		protected void ThenTheImportJobIsSuccessful(int expectedTotalRows)
		{
			Assert.That(this.TestJobResult.ErrorRows, Has.Count.Zero);
			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Zero);
			Assert.That(this.TestJobResult.CompletedJobReports, Is.All.Not.Null);
			foreach (var jobReport in this.TestJobResult.CompletedJobReports)
			{
				Assert.That(jobReport.ErrorRows, Has.Count.Zero);
				Assert.That(jobReport.FatalException, Is.Null);
			}

			var totalRecordCount = this.TestJobResult.CompletedJobReports.Select(item => item.TotalRows).Sum();
			Assert.That(totalRecordCount, Is.EqualTo(expectedTotalRows));
		}

		protected void ThenTheImportJobFailedWithFatalError(int expectedErrorRows, int expectedTotalRows)
		{
			// Note: the exact number of expected rows can vary over a range when expecting an error.
			Assert.That(this.TestJobResult.CompletedJobReports, Is.All.Not.Null);
			var totalRecordCount = this.TestJobResult.CompletedJobReports.Select(item => item.TotalRows).Sum();

			Assert.That(totalRecordCount, Is.Positive.And.LessThanOrEqualTo(expectedTotalRows));

			foreach (var jobReport in this.TestJobResult.CompletedJobReports)
			{
				Assert.That(jobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
				Assert.That(jobReport.ErrorRows, Has.Count.EqualTo(this.TestJobResult.ErrorRows.Count));
				Assert.That(jobReport.FatalException, Is.Not.Null);
			}

			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Positive);
		}

		protected void ThenTheImportJobCompletedWithErrors(int expectedErrorRows, int expectedTotalRows)
		{
			Assert.That(this.TestJobResult.CompletedJobReports, Is.All.Not.Null);
			var totalRecordCount = this.TestJobResult.CompletedJobReports.Select(item => item.TotalRows).Sum();

			Assert.That(totalRecordCount, Is.EqualTo(expectedTotalRows));

			foreach (var jobReport in this.TestJobResult.CompletedJobReports)
			{
				Assert.That(jobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
				Assert.That(jobReport.ErrorRows, Has.Count.EqualTo(this.TestJobResult.ErrorRows.Count));
				Assert.That(jobReport.FatalException, Is.Not.Null);
			}

			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Zero);
		}
	}
}