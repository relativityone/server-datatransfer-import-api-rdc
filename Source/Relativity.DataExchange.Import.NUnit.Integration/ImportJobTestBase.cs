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

		protected ImportAPI ImportAPI { get; set; }

		protected TempDirectory2 TempDirectory { get; private set; }

		[SetUp]
		public void Setup()
		{
			AppSettingsManager.Default(AppSettings.Instance);

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
	}
}