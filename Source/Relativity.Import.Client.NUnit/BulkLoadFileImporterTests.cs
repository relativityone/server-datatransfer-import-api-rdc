// -----------------------------------------------------------------------------------------------------
// <copyright file="BulkLoadFileImporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS;
	using kCura.WinEDDS.Service;

	using Moq;

	using Relativity.Import.Export;
	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Process;
	using Relativity.Logging;

	/// <summary>
	/// Represents <see cref="BulkLoadFileImporter"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class BulkLoadFileImporterTests
	{
		private Mock<IProcessEventWriter> mockProcessEventWriter;
		private Mock<IProcessErrorWriter> mockProcesErrorWriter;
		private Mock<IAppSettings> mockAppSettings;
		private Mock<IBulkImportManager> mockBulkImportManager;
		private Mock<ILog> mockLogger;
		private LoadFile args;
		private Guid guid;
		private ProcessContext context;
		private IIoReporter ioReporter;
		private MockBulkLoadFileImporter importer;
		private CancellationTokenSource tokenSource;

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

		[SetUp]
		public void Setup()
		{
			this.mockBulkImportManager = new Mock<IBulkImportManager>();
			this.args = new LoadFile();
			this.args.CaseInfo = new CaseInfo();
			this.args.CaseInfo.RootArtifactID = -1;
			this.guid = new Guid("E09E18F3-D0C8-4CFC-96D1-FBB350FAB3E1");
			this.mockAppSettings = new Mock<IAppSettings>();
			this.mockAppSettings.SetupGet(x => x.IoErrorWaitTimeInSeconds).Returns(0);
			this.mockBulkImportManager = new Mock<IBulkImportManager>();
			this.mockProcesErrorWriter = new Mock<IProcessErrorWriter>();
			this.mockProcessEventWriter = new Mock<IProcessEventWriter>();
			this.mockLogger = new Mock<ILog>();
			this.context = new ProcessContext(
				this.mockProcessEventWriter.Object,
				this.mockProcesErrorWriter.Object,
				this.mockAppSettings.Object,
				this.mockLogger.Object);
			this.tokenSource = new CancellationTokenSource();
			this.ioReporter = new IoReporter(new IoReporterContext(), this.mockLogger.Object, this.tokenSource.Token);
			this.importer = new MockBulkLoadFileImporter(
				this.args,
				this.context,
				this.ioReporter,
				this.mockLogger.Object,
				0,
				false,
				false,
				this.guid,
				true,
				"S",
				this.mockBulkImportManager.Object,
				this.tokenSource,
				Relativity.ExecutionSource.Unknown);
			AppSettings.Instance.IoErrorWaitTimeInSeconds = 0;
		}

		[Test]
		public void ShouldBulkImport()
		{
			this.importer.TryBulkImport(new kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo());
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
		}

		[Test]
		public void ShouldRetrySystemExceptions()
		{
			this.mockBulkImportManager
				.Setup(
					x => x.BulkImportObjects(
						It.IsAny<int>(),
						It.IsAny<kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo>(),
						It.IsAny<bool>())).Throws(new InvalidOperationException("bombed out"));
			InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
				() => this.importer.TryBulkImport(new kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo()));
			Assert.That(exception.Message, Is.EqualTo("bombed out"));
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
			Assert.That(this.importer.PauseCalled, Is.EqualTo(2));
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
			kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo[] fields = null;
			LoadFileFieldMap fieldMap = new LoadFileFieldMap();
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field1", 1, (int)FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field2", 2, (int)FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			this.importer.FieldMap = fieldMap;

			// ImportBehavior is null because no artifact identifiers are specified.
			fields = this.importer.GetMappedFields(1, new List<int>());
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
			Assert.That(fields[0].ImportBehavior, Is.EqualTo(ImportBehaviorChoice.ObjectFieldContainsArtifactId));
			Assert.That(fields[1].ImportBehavior, Is.Null);

			// ImportBehavior is null because none of the field types are Object or Objects.
			fieldMap = new LoadFileFieldMap();
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field01", 1, (int)FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field02", 2, (int)FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field03", 3, (int)FieldTypeHelper.FieldType.Boolean, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field04", 4, (int)FieldTypeHelper.FieldType.Code, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field05", 5, (int)FieldTypeHelper.FieldType.Currency, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field06", 6, (int)FieldTypeHelper.FieldType.Date, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field07", 7, (int)FieldTypeHelper.FieldType.Decimal, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field08", 8, (int)FieldTypeHelper.FieldType.Empty, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field09", 9, (int)FieldTypeHelper.FieldType.File, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field10", 10, (int)FieldTypeHelper.FieldType.Integer, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field11", 11, (int)FieldTypeHelper.FieldType.LayoutText, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field12", 12, (int)FieldTypeHelper.FieldType.MultiCode, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field13", 13, (int)FieldTypeHelper.FieldType.Text, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field14", 14, (int)FieldTypeHelper.FieldType.User, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field15", 15, (int)FieldTypeHelper.FieldType.Varchar, 1, 0, 0, 0, true, null, false), 0));
			this.importer.FieldMap = fieldMap;
			fields = this.importer.GetMappedFields(1, new List<int> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
			foreach (kCura.EDDS.WebAPI.BulkImportManagerBase.FieldInfo field in fields)
			{
				Assert.That(field.ImportBehavior, Is.Null);
			}

			fieldMap = new LoadFileFieldMap();
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field01", 1, (int)FieldTypeHelper.FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field02", 2, (int)FieldTypeHelper.FieldType.Objects, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field03", 3, (int)FieldTypeHelper.FieldType.Boolean, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field04", 4, (int)FieldTypeHelper.FieldType.Code, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field05", 5, (int)FieldTypeHelper.FieldType.Currency, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field06", 6, (int)FieldTypeHelper.FieldType.Date, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field07", 7, (int)FieldTypeHelper.FieldType.Decimal, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field08", 8, (int)FieldTypeHelper.FieldType.Empty, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field09", 9, (int)FieldTypeHelper.FieldType.File, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field10", 10, (int)FieldTypeHelper.FieldType.Integer, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field11", 11, (int)FieldTypeHelper.FieldType.LayoutText, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field12", 12, (int)FieldTypeHelper.FieldType.MultiCode, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field13", 13, (int)FieldTypeHelper.FieldType.Text, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field14", 14, (int)FieldTypeHelper.FieldType.User, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field15", 15, (int)FieldTypeHelper.FieldType.Varchar, 1, 0, 0, 0, true, null, false), 0));
			this.importer.FieldMap = fieldMap;
			fields = this.importer.GetMappedFields(1, new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
			for (int i = 0; i < fieldMap.Count; i++)
			{
				if (i < 2)
				{
					Assert.That(fields[i].ImportBehavior, Is.EqualTo(ImportBehaviorChoice.ObjectFieldContainsArtifactId));
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
			int actual = 0;
			System.Text.Encoding encoding = null;
			actual = this.importer.GetMaxExtractedTextLength(encoding);
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
	}
}