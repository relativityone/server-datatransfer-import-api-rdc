// -----------------------------------------------------------------------------------------------------
// <copyright file="BulkLoadFileImporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Net;
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Service;

	using Moq;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.WebApiVsKeplerSwitch;
	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents <see cref="BulkLoadFileImporter"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class BulkLoadFileImporterTests : BulkFileImporterTestsBase
	{
		private LoadFile args;
		private MockBulkLoadFileImporter importer;

		public static IEnumerable OverlayTypeSource
		{
			get
			{
				yield return new TestCaseData(LoadFile.FieldOverlayBehavior.UseRelativityDefaults, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.UseRelativityDefaults);
				yield return new TestCaseData(LoadFile.FieldOverlayBehavior.ReplaceAll, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.ReplaceAll);
				yield return new TestCaseData(LoadFile.FieldOverlayBehavior.MergeAll, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.MergeAll);
				yield return new TestCaseData(null, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior.UseRelativityDefaults);
			}
		}

		[Test]
		[TestCase("", true, true)]
		[TestCase("/some_file.txt", true, true)]
		[TestCase("com1.txt", true, true)]
		[TestCase("", false, true)]
		[TestCase("", true, false)]
		[TestCase("", false, false)]
		public void MetadataFileCountShouldBe0IfBatchCounterIs0(string outputNativePath, bool shouldCompleteJob, bool lastRun)
		{
			this.importer.PushNativeBatchInvoker(outputNativePath, shouldCompleteJob, lastRun);
			Assert.AreEqual(0, this.importer.GetMetadataFilesCount);
		}

		[Test]
		[TestCase("/some_file.txt", true, true)]
		[TestCase("/some_file.txt", false, true)]
		[TestCase("/some_file.txt", true, false)]
		[TestCase("/some_file.txt", false, false)]
		public void MetadataFileCountShouldBe0IfBatchCounterIsNot0(string outputNativePath, bool shouldCompleteJob, bool lastRun)
		{
			TapiBridgeFactory.UseLegacyWebApiInTests = false;

			this.Setup();
			AppSettings.Instance.HttpErrorNumberOfRetries = 1;
			AppSettings.Instance.HttpErrorWaitTimeInSeconds = 1;
			this.importer.SetTapiBridges();
			this.importer.SetBatchCounter(20);

			try
			{
				this.importer.PushNativeBatchInvoker(outputNativePath, shouldCompleteJob, lastRun);
			}
			catch (WebException)
			{
				// This exception is thrown when when WebAPI is used. RelativityManagerService.GetRelativityUrl
				// tries to access WebAPI and it fails.
			}
			catch (TransferException ex) when (ex.InnerException?.InnerException is WebException)
			{
				// This exception is thrown when Kepler services are used. KeplerRelativityManagerService.GetRelativityUrl
				// does not make a call to a Relativity, so it does not fail. Exception is thrown when TAPI tries to
				// make a call to the Relativity.
			}
			catch (Exception ex)
			{
				Assert.Fail($"Unexpected exception was thrown: {ex.Message}");
				throw;
			}

			Assert.AreEqual(0, this.importer.GetMetadataFilesCount);
		}

		[Test]
		[TestCase("hii}", 0, 60)]
		[TestCase("hii{0:randomString}", 4423, 0)]
		[TestCase("[[--", 2223, -100)]
		[TestCase(" {1:hh}", 123123, 34)]
		[TestCase("Prime numbers less than 10: {0}, {1}, {2}, {3}", -2113, 112334)]
		[TestCase("0x{0:omg} {0:1337} {0:N}", 232, -66)]
		[TestCase("dddd MMMM", 55, 66)]
		public void WriteCodeLineToTempFileShouldNotDoubleFormat(string documentIdentifier, int codeArtifactID, int codeTypeID)
		{
			this.importer.WriteCodeLineToTempFile(documentIdentifier, codeArtifactID, codeTypeID);
		}

		[Test]
		[TestCase("hii}", "hii}i", 0, 3224, 60)]
		[TestCase("hii{0:randomString}", "hii}i", 4423, 434, 0)]
		[TestCase("{", "[[--", 2223, 1134, -100)]
		[TestCase(" {1:hh}", "hii}i", 123123, 34, 60)]
		[TestCase("hii}", "Prime numbers less than 10: {0}, {1}, {2}, {3}", -2113, 112334, 4334)]
		[TestCase("0x{0:omg} {0:1337} {0:N}", "hii}i", 232, -66, 656)]
		[TestCase("hii}", "dddd MMMM", 55, 66, 3434)]
		public void WriteObjectLineToTempFileShouldNotDoubleFormat(string ownerIdentifier, string objectName, int artifactID, int objectTypeArtifactID, int fieldID)
		{
			this.importer.WriteObjectLineToTempFile(ownerIdentifier, objectName, artifactID, objectTypeArtifactID, fieldID);
		}

		[Test]
		public void ShouldBulkImport()
		{
			kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults results =
				this.importer.TryBulkImport(new kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo());
			Assert.That(results, Is.Not.Null);
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
			Assert.That(this.importer.PauseCalled, Is.EqualTo(0));
		}

		[Test]
		public void ShouldEnsureTheBatchSizeIsLessThenTheMinimumBatchSize()
		{
			this.importer.MinimumBatch = 100;
			this.importer.BatchSize = 300;
			Assert.That(this.importer.BatchSize, Is.EqualTo(300));
			Assert.That(this.importer.MinimumBatch, Is.EqualTo(100));
			this.importer.MinimumBatch = 500;
			this.importer.BatchSize = 300;
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
			Assert.That(this.importer.MinimumBatch, Is.EqualTo(500));
		}

		[Test]
		public void ShouldGetTheMappedFields()
		{
			LoadFileFieldMap fieldMap = new LoadFileFieldMap();
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field1", 1, (int)FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field2", 2, (int)FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			this.importer.FieldMap = fieldMap;

			// ImportBehavior is null because no artifact identifiers are specified.
			var fields = this.importer.GetMappedFields(1, new List<int>());
			Assert.That(fields.Length, Is.EqualTo(2));
			Assert.That(fields[0].ImportBehavior, Is.Null);
			Assert.That(fields[1].ImportBehavior, Is.Null);

			// ImportBehavior is null because none of the fields are mapped to the specified artifact identifiers.
			fields = this.importer.GetMappedFields(1, new List<int> { 11, 22 });
			Assert.That(fields.Length, Is.EqualTo(2));
			Assert.That(fields[0].ImportBehavior, Is.Null);
			Assert.That(fields[1].ImportBehavior, Is.Null);

			// ImportBehavior is non-null because 1 of the fields is mapped to the specified artifact identifiers.
			fields = this.importer.GetMappedFields(1, new List<int> { 1, 22 });
			Assert.That(fields.Length, Is.EqualTo(2));
			Assert.That(fields[0].ImportBehavior, Is.EqualTo(kCura.EDDS.WebAPI.BulkImportManagerBase.ImportBehaviorChoice.ObjectFieldContainsArtifactId));
			Assert.That(fields[1].ImportBehavior, Is.Null);

			// ImportBehavior is null because none of the field types are Object or Objects.
			fieldMap = new LoadFileFieldMap();
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field01", 1, (int)FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field02", 2, (int)FieldType.Objects, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field03", 3, (int)FieldType.Boolean, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field04", 4, (int)FieldType.Code, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field05", 5, (int)FieldType.Currency, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field06", 6, (int)FieldType.Date, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field07", 7, (int)FieldType.Decimal, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field08", 8, (int)FieldType.Empty, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field09", 9, (int)FieldType.File, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field10", 10, (int)FieldType.Integer, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field11", 11, (int)FieldType.LayoutText, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field12", 12, (int)FieldType.MultiCode, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field13", 13, (int)FieldType.Text, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field14", 14, (int)FieldType.User, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field15", 15, (int)FieldType.Varchar, 1, 0, 0, 0, true, null, false), 0));
			this.importer.FieldMap = fieldMap;
			fields = this.importer.GetMappedFields(1, new List<int> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
			foreach (kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo field in fields)
			{
				Assert.That(field.ImportBehavior, Is.Null);
			}

			fieldMap = new LoadFileFieldMap();
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field01", 1, (int)FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field02", 2, (int)FieldType.Objects, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field03", 3, (int)FieldType.Boolean, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field04", 4, (int)FieldType.Code, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field05", 5, (int)FieldType.Currency, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field06", 6, (int)FieldType.Date, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field07", 7, (int)FieldType.Decimal, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field08", 8, (int)FieldType.Empty, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field09", 9, (int)FieldType.File, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field10", 10, (int)FieldType.Integer, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field11", 11, (int)FieldType.LayoutText, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field12", 12, (int)FieldType.MultiCode, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field13", 13, (int)FieldType.Text, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field14", 14, (int)FieldType.User, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field15", 15, (int)FieldType.Varchar, 1, 0, 0, 0, true, null, false), 0));
			this.importer.FieldMap = fieldMap;
			fields = this.importer.GetMappedFields(1, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
			for (int i = 0; i < fieldMap.Count; i++)
			{
				if (i < 2)
				{
					Assert.That(fields[i].ImportBehavior, Is.EqualTo(kCura.EDDS.WebAPI.BulkImportManagerBase.ImportBehaviorChoice.ObjectFieldContainsArtifactId));
				}
				else
				{
					Assert.That(fields[i].ImportBehavior, Is.Null);
				}
			}
		}

		[Test]
		[TestCase("", @"\")]
		[TestCase(@"\\", @"\")]
		[TestCase(@"\ႝ\", @"\ႝ")]
		[TestCase(@"aaa\\bbb\\cc", @"\aaa\bbb\cc")]
		[TestCase(@"aaa\\\\\\\\\\bbb\\cc", @"\aaa\bbb\cc")]
		[TestCase(@".\SourceCode\Mainline\EDDS\kCura.WinEDDS\Importers", @"\SourceCode\Mainline\EDDS\kCura.WinEDDS\Importers")]
		public void ShouldCleanTheDestinationFolderPath(string input, string expected)
		{
			string actual = this.importer.CleanFolderPath(input);
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void ShouldGetTheMaxExtractedTextLength()
		{
			System.Text.Encoding encoding = null;
			var actual = this.importer.GetMaxExtractedTextLength(encoding);
			Assert.That(actual, Is.EqualTo(1073741824));
			encoding = System.Text.Encoding.UTF8;
			actual = this.importer.GetMaxExtractedTextLength(encoding);
			Assert.That(actual, Is.EqualTo(1073741824));
			encoding = System.Text.Encoding.UTF32;
			actual = this.importer.GetMaxExtractedTextLength(encoding);
			Assert.That(actual, Is.EqualTo(2147483647));
		}

		[Test]
		[TestCaseSource(nameof(OverlayTypeSource))]
		public void ShouldGetTheMassImportOverlayBehavior(LoadFile.FieldOverlayBehavior? behavior, kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior expected)
		{
			kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior actual = this.importer.ConvertOverlayBehavior(behavior);
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void ShouldWaitForRetry()
		{
			int retryMax = 10;
			int attemptCount = 0;
			this.importer.WaitForRetry(
				() =>
					{
						attemptCount++;
						return false;
					},
				string.Empty,
				string.Empty,
				string.Empty,
				retryMax,
				0);
			Assert.That(attemptCount, Is.EqualTo(retryMax + 1));
			attemptCount = 0;
			this.importer.WaitForRetry(
				() =>
					{
						attemptCount++;
						return true;
					},
				string.Empty,
				string.Empty,
				string.Empty,
				retryMax,
				0);
			Assert.That(attemptCount, Is.EqualTo(1));
			int succeedCount = 6;
			attemptCount = 0;
			this.importer.WaitForRetry(
				() =>
					{
						attemptCount++;
						return attemptCount == succeedCount;
					},
				string.Empty,
				string.Empty,
				string.Empty,
				retryMax,
				0);
			Assert.That(succeedCount, Is.EqualTo(attemptCount));
		}

		protected override void OnSetup()
		{
			var webApiVsKeplerMock = new Mock<IWebApiVsKepler>();
			webApiVsKeplerMock.Setup(x => x.UseKepler()).Returns(true);
			ManagerFactory._webApiVsKeplerLazy = new Lazy<IWebApiVsKepler>(() => webApiVsKeplerMock.Object);

			this.args = new LoadFile();
			this.args.CaseInfo = new Relativity.DataExchange.Service.CaseInfo { RootArtifactID = -1 };
			this.args.Credentials = new NetworkCredential();
			this.importer = new MockBulkLoadFileImporter(
				this.args,
				this.Context,
				this.IoReporter,
				this.MockLogger.Object,
				0,
				false,
				false,
				this.Guid,
				true,
				"S",
				this.MockBulkImportManager.Object,
				this.TokenSource,
				null);
		}
	}
}