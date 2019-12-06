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
	using System.Net;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.Transfer;

	public abstract class ImportJobTestBase<TImportJob, TSettings> : IDisposable
		where TImportJob : IImportNotifier
		where TSettings : ImportSettingsBase
	{
		private readonly ImportApiSetUp<TImportJob, TSettings> importApiSetUp;

		protected ImportJobTestBase(ImportApiSetUp<TImportJob, TSettings> importApiSetUp)
		{
			importApiSetUp.ThrowIfNull(nameof(importApiSetUp));

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

		public virtual void InitializeImportApiWithUserAndPassword(TSettings settings)
		{
			this.importApiSetUp.SetUpImportApi(this.CreateImportApiWithUserAndPwd, settings);
		}

		public virtual void InitializeImportApiWithIntegratedAuthentication(TSettings settings)
		{
			this.importApiSetUp.SetUpImportApi(this.CreateImportApiWithIntegratedAuthentication, settings);
		}

		public ImportTestJobResult Execute<T>(IEnumerable<T> importData)
		{
			this.importApiSetUp.Execute(importData);

			// At this point all results should be setup
			return this.importApiSetUp.TestJobResult;
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

		protected void ThenTheImportJobIsSuccessful(int expectedTotalRows)
		{
			this.ValidateTotalRowsCount(expectedTotalRows);
			this.ValidateFatalExceptionsNotExist();
			Assert.That(this.importApiSetUp.TestJobResult.ErrorRows, Has.Count.Zero);
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.Zero);
		}

		protected void ThenTheImportJobFailedWithFatalError(int expectedErrorRows, int expectedTotalRows)
		{
			// Note: the exact number of expected rows can vary over a range when expecting an error.
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.TotalRows, Is.Positive.And.LessThanOrEqualTo(expectedTotalRows));
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(this.importApiSetUp.TestJobResult.ErrorRows.Count));
			Assert.That(this.importApiSetUp.TestJobResult.JobFatalExceptions, Has.Count.Positive);
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.FatalException, Is.Not.Null);
		}

		protected void ThenTheImportJobCompletedWithErrors(int expectedErrorRows, int expectedTotalRows)
		{
			this.ValidateTotalRowsCount(expectedTotalRows);
			this.ValidateFatalExceptionsNotExist();
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(this.importApiSetUp.TestJobResult.ErrorRows.Count));
		}

		protected virtual void ValidateTotalRowsCount(int expectedTotalRows)
		{
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));
		}

		protected virtual void ValidateFatalExceptionsNotExist()
		{
			Assert.That(this.importApiSetUp.TestJobResult.CompletedJobReport.FatalException, Is.Null);
			Assert.That(this.importApiSetUp.TestJobResult.JobFatalExceptions, Has.Count.Zero);
		}

		protected ImportAPI CreateImportApiWithUserAndPwd()
		{
			return new ImportAPI(
				AssemblySetup.TestParameters.RelativityUserName,
				AssemblySetup.TestParameters.RelativityPassword,
				AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
		}

		protected ImportAPI CreateImportApiWithIntegratedAuthentication()
		{
			return new ImportAPI(
				AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
		}

		protected TSetUp ImportApiSetUp<TSetUp>()
			where TSetUp : ImportApiSetUp<TImportJob, TSettings>
		{
			return this.importApiSetUp as TSetUp;
		}
	}
}