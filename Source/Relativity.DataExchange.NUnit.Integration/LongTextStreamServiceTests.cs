// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextStreamServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="LongTextStreamService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Moq;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="LongTextStreamService"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class LongTextStreamServiceTests : WebServiceTestsBase
	{
		private IServiceProxyFactory serviceProxyFactory;
		private ILongTextStreamService longTextStreamService;
		private Mock<IServiceNotification> serviceNotification;
		private TestDirectory testDirectory;
		private string controlNumber;
		private string sourceExtractedTextFile;
		private int objectArtifactId;
		private int fieldArtifactId;
		private TestDirectoryManager testDirectoryManager;

		[TestCase(1)]
		[TestCase(4)]
		[TestCase(8)]
		[IdentifiedTest("9A0B34BA-6DCE-4F81-8D0C-E1C194B9AE86")]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.Export)]
		public async Task ShouldSaveTheLongTextStreamAsTextFileAsync(int taskCount)
		{
			// ARRANGE
			ConcurrentBag<LongTextStreamProgressEventArgs> progressArgs =
				new ConcurrentBag<LongTextStreamProgressEventArgs>();
			Progress<LongTextStreamProgressEventArgs> progress = new Progress<LongTextStreamProgressEventArgs>(
				args => { progressArgs.Add(args); });
			this.ArrangeLongTextImport();
			const int MaxIterations = 1;

			// ACT
			List<LongTextStreamResult> results = new List<LongTextStreamResult>();
			for (int i = 0; i < MaxIterations; i++)
			{
				results.AddRange(
					await Task.WhenAll(
						Enumerable.Range(0, taskCount).Select(
							j =>
								{
									return Task.Run(
										() => this.longTextStreamService.SaveLongTextStreamAsync(
											this.SetupLongTextStreamRequest(),
											this.CancellationTokenSource.Token,
											progress));
								})).ConfigureAwait(false));
			}

			// ASSERT
			long targetFileLength = new System.IO.FileInfo(this.sourceExtractedTextFile).Length;
			foreach (LongTextStreamResult result in results)
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Request, Is.Not.Null);
				Assert.That(result.Length, Is.EqualTo(targetFileLength));
				Assert.That(result.File, Is.Not.Null.Or.Empty);
				Assert.That(result.FileName, Is.Not.Null.Or.Empty);
				Assert.That(result.Issue, Is.Null);
				Assert.That(result.Request, Is.Not.Null);
				FileAssert.AreEqual(this.sourceExtractedTextFile, result.File);
			}

			List<LongTextStreamProgressEventArgs> expectedLastProgressArgs =
				progressArgs.Where(x => x.Completed).ToList();
			Assert.That(expectedLastProgressArgs.Count, Is.EqualTo(taskCount * MaxIterations));
			Assert.That(expectedLastProgressArgs.Select(x => x.TotalBytesWritten), Is.All.EqualTo(targetFileLength));
		}

		[IdentifiedTestCase("9F6C2005-2CB6-437F-9A71-B12805036556")]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.Export)]
		public async Task ShouldNotThrowOnInvalidSourceObjectArtifactAsync()
		{
			// ARRANGE
			this.ArrangeLongTextImport();
			LongTextStreamRequest request = this.SetupLongTextStreamRequest();
			request.SourceObjectArtifactId = int.MaxValue;

			// ACT
			LongTextStreamResult result = await Task.Run(
											  () => this.longTextStreamService.SaveLongTextStreamAsync(
												  request,
												  this.CancellationTokenSource.Token,
												  null)).ConfigureAwait(false);

			// ASSERT
			VerifyTheNonFatalLongTextResult(request, result, 0);
		}

		[IdentifiedTestCase("14D88D1E-AC49-4A6C-AB31-11F91982BBAB")]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.Export)]
		public async Task ShouldNotThrowOnInvalidSourceFieldArtifactAsync()
		{
			// ARRANGE
			this.ArrangeLongTextImport();
			LongTextStreamRequest request = this.SetupLongTextStreamRequest();
			request.SourceFieldArtifactId = int.MaxValue;

			// ACT
			LongTextStreamResult result = await Task.Run(
											  () => this.longTextStreamService.SaveLongTextStreamAsync(
												  request,
												  this.CancellationTokenSource.Token,
												  null)).ConfigureAwait(false);

			// ASSERT
			VerifyTheNonFatalLongTextResult(request, result, 0);
		}

		protected override void OnSetup()
		{
			this.objectArtifactId = 0;
			this.fieldArtifactId = 0;
			this.controlNumber = "REL-" + Guid.NewGuid();
			this.testDirectory = new TestDirectory();
			this.testDirectory.Create();
			this.testDirectoryManager = new TestDirectoryManager(this.testDirectory.Directory);
			this.sourceExtractedTextFile = System.IO.Path.Combine(this.testDirectory.Directory, "source-extracted-text.txt");
			this.serviceProxyFactory = new KeplerServiceProxyFactory(
				new KeplerServiceConnectionInfo(
					this.TestParameters.RelativityWebApiUrl,
					new NetworkCredential(
						this.TestParameters.RelativityUserName,
						this.TestParameters.RelativityPassword)));
			this.AppSettings.ExportLongTextLargeFileProgressRateSeconds = 0;
			this.AppSettings.HttpErrorWaitTimeInSeconds = 12;
			this.AppSettings.HttpErrorNumberOfRetries = 5;
			this.serviceNotification = new Mock<IServiceNotification>();
			this.longTextStreamService = new LongTextStreamService(
				this.serviceProxyFactory,
				new KeplerRetryPolicyFactory(this.AppSettings),
				this.serviceNotification.Object,
				this.AppSettings,
				FileSystem.Instance,
				this.Logger.Object);
		}

		protected override void OnTeardown()
		{
			base.OnTeardown();
			this.longTextStreamService?.Dispose();
			this.serviceProxyFactory?.Dispose();
			this.testDirectory?.Dispose();
		}

		private static void VerifyTheNonFatalLongTextResult(
			LongTextStreamRequest request,
			LongTextStreamResult result,
			int expectedRetryCount)
		{
			Assert.That(result.Request, Is.Not.Null);
			Assert.That(result.Request, Is.SameAs(request));
			Assert.That(result.Issue, Is.Not.Null);
			Assert.That(result.Length, Is.Zero);
			Assert.That(result.File, Is.Null.Or.Empty);
			Assert.That(result.FileName, Is.Null.Or.Empty);
			Assert.That(result.RetryCount, Is.EqualTo(expectedRetryCount));
		}

		private void ArrangeLongTextImport()
		{
			this.AutoGenerateSourceLongTextFile();
			this.ImportOneDocWithSourceLongTextFile();
			this.RetrieveLongTextArtifacts();
		}

		private void AutoGenerateSourceLongTextFile()
		{
			const int OneMegaByte = 1048576;
			long length = RandomHelper.NextInt64(OneMegaByte, 2 * OneMegaByte);
			RandomHelper.NextLargeTextFile(Encoding.Unicode, length, this.sourceExtractedTextFile);
		}

		private void ImportOneDocWithSourceLongTextFile()
		{
			var iapi = new ImportAPI(
				this.TestParameters.RelativityUserName,
				this.TestParameters.RelativityPassword,
				this.TestParameters.RelativityWebApiUrl.ToString());
			ImportBulkArtifactJob job = iapi.NewNativeDocumentImportJob();
			Settings settings = job.Settings;
			settings.ArtifactTypeId = 10;
			settings.CaseArtifactId = this.TestParameters.WorkspaceId;
			settings.ExtractedTextFieldContainsFilePath = true;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.FileSizeMapped = false;
			settings.FileSizeColumn = null;
			settings.NativeFilePathSourceFieldName = null;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.OIFileIdMapped = false;
			settings.OIFileIdColumnName = null;
			settings.OIFileTypeColumnName = null;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.StartRecordNumber = 0;
			settings.WebServiceURL = this.TestParameters.RelativityWebApiUrl.ToString();
			using (var dataSource = new System.Data.DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.Columns.Add(WellKnownFields.ControlNumber);
				dataSource.Columns.Add(WellKnownFields.ExtractedText);
				dataSource.Columns.Add(WellKnownFields.FolderName);
				dataSource.Rows.Add(this.controlNumber, this.sourceExtractedTextFile, string.Empty);
				job.SourceData.SourceData = dataSource.CreateDataReader();
				job.OnFatalException += report => throw report.FatalException;
				job.Execute();
			}
		}

		private void RetrieveLongTextArtifacts()
		{
			string condition = $"'{WellKnownFields.ControlNumber}' == '{this.controlNumber}'";
			IList<RelativityObject> rdos = RdoHelper.QueryRelativityObjects(
				this.TestParameters,
				WellKnownArtifactTypes.DocumentArtifactTypeId,
				condition,
				new[] { WellKnownFields.ControlNumber, WellKnownFields.ExtractedText });
			if (rdos.Count != 1)
			{
				Assert.Fail("The test workspace must include at least 1 document.");
			}

			RelativityObject rdo = rdos.First();
			FieldValuePair pair = rdo.FieldValues
				.FirstOrDefault(x => x.Field.Name == WellKnownFields.ExtractedText);
			if (pair == null)
			{
				Assert.Fail($"The test document must include the '{WellKnownFields.ExtractedText}' extracted text field.");
			}

			this.objectArtifactId = rdo.ArtifactID;
			this.fieldArtifactId = pair.Field.ArtifactID;
		}

		private LongTextStreamRequest SetupLongTextStreamRequest()
		{
			return new LongTextStreamRequest
			{
				SourceEncoding = Encoding.Unicode,
				SourceObjectArtifactId = this.objectArtifactId,
				SourceFieldArtifactId = this.fieldArtifactId,
				TargetFile =
							   this.testDirectoryManager.MoveNext($"target-extracted-text-{Guid.NewGuid()}.txt"),
				TargetEncoding = Encoding.Unicode,
				WorkspaceId = this.TestParameters.WorkspaceId,
			};
		}
	}
}