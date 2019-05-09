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

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.Import.Export.Service;

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
		public void ShouldBulkImport()
		{
			kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults results =
				this.importer.TryBulkImport(new kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo());
			Assert.That(results, Is.Not.Null);
			Assert.That(this.importer.BatchSize, Is.EqualTo(500));
			Assert.That(this.importer.PauseCalled, Is.EqualTo(0));
		}

		[Test]
		public void ShouldRetrySystemExceptions()
		{
			this.MockBulkImportManager
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
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field1", 1, (int)FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
			fieldMap.Add(new LoadFileFieldMap.LoadFileFieldMapItem(new DocumentField("Field2", 2, (int)FieldType.Object, 1, 0, 0, 0, true, null, false), 0));
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

		protected override void OnSetup()
		{
			this.args = new LoadFile();
			this.args.CaseInfo = new Relativity.Import.Export.Service.CaseInfo();
			this.args.CaseInfo.RootArtifactID = -1;
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
				ExecutionSource.Unknown);
		}
	}
}