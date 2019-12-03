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

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	public abstract class ImportJobTestBase<TImportJob, TSettings> : IDisposable
		where TImportJob : IImportNotifier
		where TSettings : ImportSettingsBase
	{
		private ImportApiSetUp<TImportJob, TSettings> importApiSetUp;

		protected ImportJobTestBase(ImportApiSetUp<TImportJob, TSettings> importApiSetUp)
		{
			Assume.That(AssemblySetup.TestParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;

			this.importApiSetUp = importApiSetUp;
		}

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

			this.importApiSetUp.Dispose();
		}

		public virtual void CreateImportApiSetUpWithUserAndPwd(TSettings settings)
		{
			this.ImportApiSetUp = this.CreateImportApiSetUp().SetUpImportApi(this.CreateImportApiWithUserAndPwd(), settings);
		}

		public virtual void CreateImportApiSetUpWithIntegratedAuthentication(TSettings settings)
		{
			this.ImportApiSetUp = this.CreateImportApiSetUp().SetUpImportApi(this.CreateImportApiWithIntegratedAuthentication(), settings);
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

		protected abstract ImportApiSetUp<TImportJob, TSettings> CreateImportApiSetUp();

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Teardown();
			}
		}

		protected void ThenTheImportJobIsSuccessful(int expectedTotalRows)
		{
			Assert.That(this.ImportApiSetUp.TestJobResult.ErrorRows, Has.Count.Zero);
			Assert.That(this.ImportApiSetUp.TestJobResult.JobFatalExceptions, Has.Count.Zero);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.Zero);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.FatalException, Is.Null);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));
		}

		protected void ThenTheImportJobFailedWithFatalError(int expectedErrorRows, int expectedTotalRows)
		{
			// Note: the exact number of expected rows can vary over a range when expecting an error.
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.TotalRows, Is.Positive.And.LessThanOrEqualTo(expectedTotalRows));

			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(this.ImportApiSetUp.TestJobResult.ErrorRows.Count));

			Assert.That(this.ImportApiSetUp.TestJobResult.JobFatalExceptions, Has.Count.Positive);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.FatalException, Is.Not.Null);
		}

		protected void ThenTheImportJobCompletedWithErrors(int expectedErrorRows, int expectedTotalRows)
		{
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));

			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(this.ImportApiSetUp.TestJobResult.ErrorRows.Count));

			Assert.That(this.ImportApiSetUp.TestJobResult.JobFatalExceptions, Has.Count.Zero);
			Assert.That(this.ImportApiSetUp.TestJobResult.CompletedJobReport.FatalException, Is.Null);
		}

		private ImportAPI CreateImportApiWithUserAndPwd()
		{
			return new ImportAPI(
				AssemblySetup.TestParameters.RelativityUserName,
				AssemblySetup.TestParameters.RelativityPassword,
				AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
		}

		private ImportAPI CreateImportApiWithIntegratedAuthentication()
		{
			return new ImportAPI(
				AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
		}
	}
}