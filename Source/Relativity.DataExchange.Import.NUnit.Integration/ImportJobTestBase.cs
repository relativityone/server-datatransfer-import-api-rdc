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
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;
	using kCura.Relativity.DataReaderClient;

	using Newtonsoft.Json.Linq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Interfaces.Field;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;

	public abstract class ImportJobTestBase<TJobExecutionContext> : IDisposable
		where TJobExecutionContext : class, IDisposable, new()
	{
		private const int MaxLoggedErrors = 100;

		protected ImportJobTestBase()
			: this(AssemblySetup.TestParameters)
		{
		}

		private ImportJobTestBase(IntegrationTestParameters testParameters)
		{
			this.SetTestParameters(testParameters);

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
																			 | SecurityProtocolType.Tls11
																			 | SecurityProtocolType.Tls12;
		}

		protected IntegrationTestParameters TestParameters { get; private set; }

		protected TJobExecutionContext JobExecutionContext { get; private set; }

		protected TempDirectory2 TempDirectory { get; private set; }

		[SetUp]
		public async Task SetupAsync()
		{
			TapiClientModeAvailabilityChecker.InitializeTapiClient(this.TestParameters);

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

		protected static void ThenTheErrorRowsHaveCorrectMessage(
			IEnumerable<IDictionary> errorRows,
			string expectedMessage)
		{
			ThenCheckCorrectMessage(errorRows, expectedMessage, StringAssert.AreEqualIgnoringCase);
		}

		protected static void ThenTheErrorRowsContainsCorrectMessage(
			IEnumerable<IDictionary> errorRows,
			string expectedMessage)
		{
			ThenCheckCorrectMessage(errorRows, expectedMessage, StringAssert.Contains);
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

			Assert.That(
				testJobResult.SwitchedToWebMode,
				Is.False,
				$"Job was expected to run in {expectedClient} mode but switched to Web mode");
		}

		protected static IEnumerable<string> GetControlNumberEnumerable(
			OverwriteModeEnum overwriteMode,
			int numberOfDocumentsToAppend,
			string nameSuffix)
		{
			IEnumerable<string> controlNumber =
				(overwriteMode == OverwriteModeEnum.Overlay || overwriteMode == OverwriteModeEnum.AppendOverlay)
					? TestData.SampleDocFiles.Select(Path.GetFileName)
					: Enumerable.Empty<string>();

			if (overwriteMode == OverwriteModeEnum.Append || overwriteMode == OverwriteModeEnum.AppendOverlay)
			{
				controlNumber = controlNumber.Concat(GetIdentifiersEnumerable(numberOfDocumentsToAppend, nameSuffix));
			}

			return controlNumber;
		}

		protected static IEnumerable<string> GetIdentifiersEnumerable(
			int numberOfDocumentsToAppend,
			string nameSuffix)
		{
			return Enumerable.Range(1, numberOfDocumentsToAppend).Select(p => $"{p}-{nameSuffix}");
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
			await FieldHelper
				.CreateFileFieldAsync(this.TestParameters, WellKnownFields.FilePath, objectId).ConfigureAwait(false);

			int artifactId =
				FieldHelper.QueryIdentifierFieldId(this.TestParameters, objectName);

			if (RelativityVersionChecker.VersionIsLowerThan(this.TestParameters, RelativityVersion.Goatsbeard))
			{
				await this.UpdateFixedLengthFieldUsingHttpClientAsync(objectName, artifactId).ConfigureAwait(false);
			}
			else
			{
				await this.UpdateFixedLengthFieldUsingKeplerAsync(objectName, artifactId).ConfigureAwait(false);
			}

			return objectId;
		}

		protected void
			ThenTheImportJobIsSuccessful(
				ImportTestJobResult testJobResult,
				int expectedTotalRows) // TODO create extension method for that
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			this.ValidateFatalExceptionsNotExist(testJobResult);
			this.ValidateTotalRowsCount(testJobResult, expectedTotalRows);
			this.ValidateErrorRowsCount(testJobResult, 0);
		}

		protected void ThenTheImportJobFailedWithFatalError(
			ImportTestJobResult testJobResult,
			int expectedErrorRows,
			int expectedTotalRows)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			// Note: the exact number of expected rows can vary over a range when expecting an error.
			Assert.That(testJobResult.JobReportTotalRows, Is.Positive.And.LessThanOrEqualTo(expectedTotalRows));
			this.ValidateErrorRowsCount(testJobResult, expectedErrorRows);
			Assert.That(testJobResult.JobFatalExceptions, Has.Count.Positive);
			Assert.That(testJobResult.FatalException, Is.Not.Null);
		}

		protected void ThenTheImportJobCompletedWithErrors(
			ImportTestJobResult testJobResult,
			int expectedErrorRows,
			int expectedTotalRows)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			this.ValidateFatalExceptionsNotExist(testJobResult);
			this.ValidateTotalRowsCount(testJobResult, expectedTotalRows);
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

			Assert.That(testJobResult.JobReportErrorsCount, Is.EqualTo(expectedErrorRows), () => this.GetFailureMessageIfNumberOfErrorsDifferentThanExpected(testJobResult));
			Assert.That(testJobResult.ErrorRows.Count, Is.EqualTo(expectedErrorRows), () => this.GetFailureMessageIfNumberOfErrorsDifferentThanExpected(testJobResult));
		}

		protected string GetFailureMessageIfNumberOfErrorsDifferentThanExpected(ImportTestJobResult testJobResult)
		{
			var builder = new StringBuilder();
			builder.AppendLine("Number of errors was different than expected.");

			if (testJobResult?.ErrorRows?.Count() > 0)
			{
				foreach (var errorRow in testJobResult.ErrorRows.Take(MaxLoggedErrors))
				{
					builder.AppendLine(string.Join(string.Empty, errorRow.Values.OfType<string>()));
				}
			}

			return builder.ToString();
		}

		protected virtual void ValidateFatalExceptionsNotExist(ImportTestJobResult testJobResult)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));

			Assert.That(
				testJobResult.FatalException,
				Is.Null,
				$"Import was aborted due to the fatal exception: {testJobResult.FatalException?.Message}.");
			Assert.That(
				testJobResult.JobFatalExceptions,
				Has.Count.Zero,
				$"{testJobResult.JobFatalExceptions.Count} fatal exceptions were thrown during import.");
		}

		protected async Task ThenTheAuditIsCorrectAsync(int userId, DateTime executionTime, List<Dictionary<string, string>> expectedAuditsDetails, int nrOfLastAuditsToTake, AuditHelper.AuditAction action)
		{
			var auditsDetails = await AuditHelper.GetLastAuditDetailsForActionAsync(this.TestParameters, action, executionTime, nrOfLastAuditsToTake, userId)
				                   .ConfigureAwait(false);

			var actualAuditsDetails = auditsDetails.ToList();

			Assert.AreEqual(expectedAuditsDetails.Count, actualAuditsDetails.Count);

			for (int i = 0; i < actualAuditsDetails.Count; i++)
			{
				var expectedAuditDetails = expectedAuditsDetails[i];
				var actualAuditDetails = actualAuditsDetails[i];

				foreach (string key in expectedAuditDetails.Keys)
				{
					Assert.AreEqual(expectedAuditDetails[key], actualAuditDetails[key], $"Audit verification failed for field '{key}'");
				}
			}
		}

		private static void ThenCheckCorrectMessage(
			IEnumerable<IDictionary> errorRows,
			string expectedMessage,
			Action<string, string> validationAction)
		{
			errorRows = errorRows ?? throw new ArgumentNullException(nameof(errorRows));

			foreach (var row in errorRows)
			{
				string actualMessage = (string)row["Message"];
				validationAction(expectedMessage, actualMessage);
			}
		}

		private static async Task<string> PrepareUpdateFixedLengthFieldRequestAsync(IntegrationTestParameters testParameters, string objectName, int artifactId)
		{
			var url =
				$"{testParameters.RelativityRestUrl.AbsoluteUri}/Relativity.Fields/workspace/{testParameters.WorkspaceId}/fields/{artifactId}";

			JObject objectData = JObject.Parse(await HttpClientHelper.GetAsync(testParameters, new Uri(url)).ConfigureAwait(false));

			var updateFixedLengthField = ResourceFileHelper.GetResourceFolderPath("UpdateFixedLengthTextFieldRequest.json");
			JObject request = JObject.Parse(File.ReadAllText(updateFixedLengthField));

			request["fieldRequest"]["Name"] = WellKnownFields.ControlNumber;
			request["fieldRequest"]["ObjectType"]["Name"] = objectName;
			request["fieldRequest"]["ObjectType"]["ArtifactID"] = objectData["ObjectType"]["ArtifactID"];
			request["fieldRequest"]["ObjectType"]["ArtifactTypeID"] = objectData["ObjectType"]["ArtifactTypeID"];

			return request.ToString();
		}

		private void SetTestParameters(IntegrationTestParameters testParameters)
		{
			this.TestParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));
			Assume.That(
				testParameters.WorkspaceId,
				Is.Positive,
				"The test workspace must be created or specified in order to run this integration test. One possible reason for this error is that Skip Integration Tests is set to true.");
		}

		private async Task UpdateFixedLengthFieldUsingKeplerAsync(string objectName, int queryFieldId)
		{
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
					queryFieldId,
					controlNumberFieldRequest).ConfigureAwait(false);
			}
		}

		private async Task UpdateFixedLengthFieldUsingHttpClientAsync(string objectName, int artifactId)
		{
			string request =
				await PrepareUpdateFixedLengthFieldRequestAsync(this.TestParameters, objectName, artifactId)
					.ConfigureAwait(false);

			var url =
				$"{this.TestParameters.RelativityRestUrl.AbsoluteUri}/relativity.fields/workspace/{this.TestParameters.WorkspaceId}/fixedlengthfields/{artifactId}";

			await HttpClientHelper.PutAsync(this.TestParameters, new Uri(url), request.ToString())
							 .ConfigureAwait(false);
		}
	}
}