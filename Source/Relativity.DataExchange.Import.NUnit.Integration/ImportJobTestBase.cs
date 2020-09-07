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
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;

	using global::NUnit.Framework;
	using kCura.Relativity.DataReaderClient;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Interfaces.Field;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;

	using FieldHelper = Relativity.FieldHelper;

	public abstract class ImportJobTestBase<TJobExecutionContext> : IDisposable
		where TJobExecutionContext : class, IDisposable, new()
	{
		protected ImportJobTestBase()
			: this(AssemblySetup.TestParameters)
		{
		}

		private ImportJobTestBase(IntegrationTestParameters testParameters)
		{
			this.SetTestParameters(testParameters);

			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
		}

		protected IntegrationTestParameters TestParameters { get; private set; }

		protected TJobExecutionContext JobExecutionContext { get; private set; }

		protected TempDirectory2 TempDirectory { get; private set; }

		[SetUp]
		public async Task SetupAsync()
		{
			TapiClientModeAvailabilityChecker.SkipTestIfTestParameterTransferModeNotAvailable(this.TestParameters);

			await this.OnSetUpAsync().ConfigureAwait(false);
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

			this.JobExecutionContext = new TJobExecutionContext();
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

			this.JobExecutionContext?.Dispose();
			this.JobExecutionContext = null;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public async Task ResetContextAsync()
		{
			IntegrationTestParameters newParameters = await AssemblySetup.ResetContextAsync().ConfigureAwait(false);
			this.SetTestParameters(newParameters);
		}

		protected static void ThenTheErrorRowsHaveCorrectMessage(IEnumerable<IDictionary> errorRows, string expectedMessage)
		{
			errorRows = errorRows ?? throw new ArgumentNullException(nameof(errorRows));

			foreach (var row in errorRows)
			{
				string actualMessage = (string)row["Message"];
				StringAssert.AreEqualIgnoringCase(expectedMessage, actualMessage);
			}
		}

		protected static void ThenTheJobCompletedInCorrectTransferMode(
			ImportTestJobResult testJobResult,
			TapiClient expectedClient)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			if (expectedClient == TapiClient.None)
			{
				return;
			}

			Assert.That(testJobResult.SwitchedToWebMode, Is.False, $"Job was expected to run in {expectedClient} mode but switched to Web mode");
		}

		protected static IEnumerable<string> GetControlNumberEnumerable(
			OverwriteModeEnum overwriteMode,
			int numberOfDocumentsToAppend,
			string nameSuffix)
		{
			IEnumerable<string> controlNumber = (overwriteMode == OverwriteModeEnum.Overlay || overwriteMode == OverwriteModeEnum.AppendOverlay) ?
				                                    TestData.SampleDocFiles.Select(Path.GetFileName) :
				                                    Enumerable.Empty<string>();

			if (overwriteMode == OverwriteModeEnum.Append || overwriteMode == OverwriteModeEnum.AppendOverlay)
			{
				controlNumber = controlNumber.Concat(
					Enumerable.Range(1, numberOfDocumentsToAppend).Select(p => $"{p}-{nameSuffix}"));
			}

			return controlNumber;
		}

		protected static void ForceClient(TapiClient tapiClient)
		{
			AppSettings.Instance.TapiForceAsperaClient = tapiClient == TapiClient.Aspera;
			AppSettings.Instance.TapiForceFileShareClient = tapiClient == TapiClient.Direct;
			AppSettings.Instance.TapiForceHttpClient = tapiClient == TapiClient.Web;
		}

		protected virtual Task OnSetUpAsync()
		{
			return Task.CompletedTask;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Teardown();
			}
		}

		protected async Task<int> CreateObjectInWorkspaceAsync()
		{
			string objectName = Guid.NewGuid().ToString();

			var objectId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, objectName).ConfigureAwait(false);
			await TestFramework.RelativityHelpers.FieldHelper.CreateFileFieldAsync(this.TestParameters, WellKnownFields.FilePath, objectId).ConfigureAwait(false);

			var controlNumberFieldRequest = new FixedLengthFieldRequest()
				                                {
					                                Name = WellKnownFields.ControlNumber,
					                                ObjectType = new ObjectTypeIdentifier() { Name = objectName },
					                                Length = 255,
					                                IsRequired = true,
					                                IncludeInTextIndex = true,
					                                FilterType = FilterType.TextBox,
					                                AllowSortTally = true,
					                                AllowGroupBy = false,
					                                AllowPivot = false,
					                                HasUnicode = true,
					                                OpenToAssociations = false,
					                                IsRelational = false,
					                                AllowHtml = false,
					                                IsLinked = true,
					                                Wrapping = true,
				                                };
			using (IFieldManager fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(this.TestParameters))
			{
				await fieldManager.UpdateFixedLengthFieldAsync(
					this.TestParameters.WorkspaceId,
					TestFramework.RelativityHelpers.FieldHelper.QueryIdentifierFieldId(this.TestParameters, objectName),
					controlNumberFieldRequest).ConfigureAwait(false);
			}

			return objectId;
		}

		protected void ThenTheImportJobIsSuccessful(ImportTestJobResult testJobResult, int expectedTotalRows) // TODO create extension method for that
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			this.ValidateTotalRowsCount(testJobResult, expectedTotalRows);
			this.ValidateFatalExceptionsNotExist(testJobResult);
			this.ValidateErrorRowsCount(testJobResult, 0);
		}

		protected void ThenTheImportJobFailedWithFatalError(ImportTestJobResult testJobResult, int expectedErrorRows, int expectedTotalRows)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			// Note: the exact number of expected rows can vary over a range when expecting an error.
			Assert.That(testJobResult.JobReportTotalRows, Is.Positive.And.LessThanOrEqualTo(expectedTotalRows));
			this.ValidateErrorRowsCount(testJobResult, expectedErrorRows);
			Assert.That(testJobResult.JobFatalExceptions, Has.Count.Positive);
			Assert.That(testJobResult.FatalException, Is.Not.Null);
		}

		protected void ThenTheImportJobCompletedWithErrors(ImportTestJobResult testJobResult, int expectedErrorRows, int expectedTotalRows)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			this.ValidateTotalRowsCount(testJobResult, expectedTotalRows);
			this.ValidateFatalExceptionsNotExist(testJobResult);
			this.ValidateErrorRowsCount(testJobResult, expectedErrorRows);
		}

		protected virtual void ValidateTotalRowsCount(ImportTestJobResult testJobResult, int expectedTotalRows)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			Assert.That(testJobResult.JobReportTotalRows, Is.EqualTo(expectedTotalRows));
		}

		protected virtual void ValidateJobMessagesContainsText(ImportTestJobResult testJobResult, string text)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			Assert.That(testJobResult.JobMessages, Has.Some.Contains(text));
		}

		protected virtual void ValidateErrorRowsCount(ImportTestJobResult testJobResult, int expectedErrorRows)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			string failureMessage = "Number of errors was different than expected.";
			Assert.That(testJobResult.JobReportErrorsCount, Is.EqualTo(expectedErrorRows), () => failureMessage);
			Assert.That(testJobResult.ErrorRows.Count, Is.EqualTo(expectedErrorRows), () => failureMessage);
		}

		protected virtual void ValidateFatalExceptionsNotExist(ImportTestJobResult testJobResult)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			Assert.That(testJobResult.FatalException, Is.Null);
			Assert.That(testJobResult.JobFatalExceptions, Has.Count.Zero);
		}

		private void SetTestParameters(IntegrationTestParameters testParameters)
		{
			this.TestParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));
			Assume.That(testParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");
		}
	}
}