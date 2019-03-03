// -----------------------------------------------------------------------------------------------------
// <copyright file="ProcessErrorWriterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ProcessErrorWriter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.Import.Export.Process;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="ProcessErrorReader"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class ProcessErrorWriterTests
	{
		private Mock<Relativity.Logging.ILog> logger;
		private ProcessErrorWriter writer;

		[SetUp]
		public void Setup()
		{
			this.logger = new Mock<Relativity.Logging.ILog>();
			this.writer = new ProcessErrorWriter(this.logger.Object);
		}

		[TearDown]
		public void Teardown()
		{
			this.writer?.Dispose();
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						Relativity.Logging.ILog log = this.logger.Object;
						log = null;
						using (new ProcessErrorWriter(log))
						{
						}
					});
		}

		[Test]
		[TestCase("", null)]
		[TestCase("", "")]
		[TestCase(null, null)]
		[TestCase(null, "")]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheKeyOrDescriptionIsNullOrEmpty(string key, string description)
		{
			Assert.Throws<ArgumentNullException>(() => { this.writer.Write(key, description); });
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldHaveNoErrorsWhenNoneWereWritten()
		{
			Assert.That(this.writer.HasErrors, Is.False);
		}

		[Test]
		[TestCase("1", "1-description")]
		[TestCase("2", "2-description")]
		[TestCase("3", "3-description")]
		[Category(TestCategories.Framework)]
		public void ShouldWriteTheErrorAndBuildTheReport(string key, string description)
		{
			this.writer.Write(key, description);
			Assert.That(this.writer.HasErrors, Is.True);
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldBuildTheReport()
		{
			this.writer.Write("1", "1-description");
			this.writer.Write("2", "2-description");
			this.writer.Write("3", "3-description");
			ProcessErrorReport report = this.writer.BuildErrorReport(CancellationToken.None);
			Assert.That(report, Is.Not.Null);
			Assert.That(report.MaxLengthExceeded, Is.False);
			Assert.That(report.Report, Is.Not.Null);
			Assert.That(report.Report.Rows.Count, Is.EqualTo(3));
			VerifyRow(report.Report.Rows[0], "1", "1-description");
			VerifyRow(report.Report.Rows[1], "2", "2-description");
			VerifyRow(report.Report.Rows[2], "3", "3-description");
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldSetMaxLengthExceededWhenExceedingTheMaxNumberOfRows()
		{
			for (int i = 0; i < ProcessErrorReader.MaxRows + 1; i++)
			{
				this.writer.Write($"{i + 1}", $"{i + 1}-description");
			}

			ProcessErrorReport report = this.writer.BuildErrorReport(CancellationToken.None);
			Assert.That(report, Is.Not.Null);
			Assert.That(report.MaxLengthExceeded, Is.True);
		}

		private static void VerifyRow(System.Data.DataRow row, string expectedKey, string expectedDescription)
		{
			Assert.That(row[ProcessErrorReader.ColumnKey], Is.EqualTo(expectedKey));
			Assert.That(row[ProcessErrorReader.ColumnDescription], Is.EqualTo(expectedDescription));
			Assert.That(row[ProcessErrorReader.ColumnStatus], Is.Not.Null.Or.Empty);
			Assert.That(row[ProcessErrorReader.ColumnTimestamp], Is.Not.Null.Or.Empty);
		}
	}
}