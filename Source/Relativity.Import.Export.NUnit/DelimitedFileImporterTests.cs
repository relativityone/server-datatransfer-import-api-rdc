﻿// -----------------------------------------------------------------------------------------------------
 // <copyright file="DelimitedFileImporterTests.cs" company="Relativity ODA LLC">
 //   © Relativity All Rights Reserved.
 // </copyright>
 // <summary>
 //   Represents <see cref="DelimitedFileImporter"/> tests.
 // </summary>
 // -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Resources;
	using Relativity.Import.Export.TestFramework;
	using Relativity.Logging;

	/// <summary>
	/// Represents <see cref="DelimitedFileImporter"/> tests.
	/// </summary>
	[TestFixture]
	public class DelimitedFileImporterTests
	{
		private DelimitedFileImporter importer;
		private Mock<Relativity.Logging.ILog> mockLogger;

		[SetUp]
		public void Setup()
		{
			this.importer = new MockDelimitedFileImport();
			this.mockLogger = new Mock<ILog>();
		}

		[TearDown]
		public void TearDown()
		{
			this.importer.Close();
		}

		[Test]
		public void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			// Note: going to extremes here because the constructor includes both
			//       character array and string parameters and the rules on null
			//       are very particular.
			const string NeverExecuteMessage = "This should never execute!";
			char[] nullCharDelimiter = null;
			char[] nullCharBound = null;
			char[] nullCharNewline = null;
			string nullStringDelimiter = null;
			string nullStringBound = null;
			string nullStringNewline = null;
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(nullCharDelimiter);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("delimiter"));
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(nullCharDelimiter, nullCharBound);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("delimiter"));
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(
							new[] { MockDelimitedFileImport.DefaultDelimiter },
							nullCharBound,
							nullCharNewline,
							null,
							this.mockLogger.Object,
							CancellationToken.None);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("context"));
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(
							new[] { MockDelimitedFileImport.DefaultDelimiter },
							nullCharBound,
							nullCharNewline,
							new IoReporterContext(),
							null,
							CancellationToken.None);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("logger"));
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(nullStringDelimiter);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("delimiter"));
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(nullStringDelimiter, nullStringBound);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("delimiter"));
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(
							MockDelimitedFileImport.DefaultDelimiter.ToString(),
							nullStringBound,
							nullStringNewline,
							null,
							this.mockLogger.Object,
							CancellationToken.None);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("context"));
			Assert.That(
				() =>
					{
						DelimitedFileImporter temp = new MockDelimitedFileImport(
							MockDelimitedFileImport.DefaultDelimiter.ToString(),
							nullStringBound,
							nullStringNewline,
							new IoReporterContext(),
							null,
							CancellationToken.None);
						Assert.That(temp, Is.Null, NeverExecuteMessage);
					},
				Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("logger"));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void ShouldThrowWhenGettingTheDecimalValue(bool excelSingleCharOrdinal)
		{
			int columnNumber = GetRandomColumnOrdinal(excelSingleCharOrdinal);
			string expectedErrorMessage = ImporterException.GetExcelStyleErrorMessage(
				this.importer.CurrentLineNumber,
				columnNumber,
				Strings.DecimalImporterErrorAdditionalInfo);
			Assert.That(
				() => this.importer.GetDecimal(null, columnNumber),
				Throws.TypeOf<DecimalImporterException>().With.Message.EqualTo(expectedErrorMessage).And.With
					.Property("Row").EqualTo(this.importer.CurrentLineNumber).And.With.Property("Column")
					.EqualTo(columnNumber));
			Assert.That(
				() => this.importer.GetDecimal("1000000000000000.43", columnNumber),
				Throws.TypeOf<DecimalImporterException>().With.Message.EqualTo(expectedErrorMessage).And.With
					.Property("Row").EqualTo(this.importer.CurrentLineNumber).And.With.Property("Column")
					.EqualTo(columnNumber));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void ShouldThrowWhenGettingTheNullableFixedStringThatExceedsTheMaxLength(bool excelSingleCharOrdinal)
		{
			// The Excel flag should have no bearing - but just making sure.
			int columnNumber = GetRandomColumnOrdinal(excelSingleCharOrdinal);
			string additionalInfoMessage = StringImporterException.GetAdditionalInfoMessage(5, 3, "TargetPath");
			string expectedErrorMessage = ImporterException.GetErrorMessage(
				this.importer.CurrentLineNumber,
				"TargetPath",
				additionalInfoMessage);
			Assert.That(
				() => this.importer.GetNullableFixedString("abcde", columnNumber, 3, "TargetPath"),
				Throws.TypeOf<StringImporterException>().With.Message.EqualTo(expectedErrorMessage).And.With
					.Property("Row").EqualTo(this.importer.CurrentLineNumber).And.With.Property("Column")
					.EqualTo(columnNumber));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void ShouldThrowWhenGettingTheNullableInteger(bool excelSingleCharOrdinal)
		{
			int columnNumber = GetRandomColumnOrdinal(excelSingleCharOrdinal);
			string expectedErrorMessage = ImporterException.GetExcelStyleErrorMessage(
				this.importer.CurrentLineNumber,
				columnNumber,
				Strings.IntegerImporterErrorAdditionalInfo);
			Assert.That(
				() => this.importer.GetNullableInteger("abc", columnNumber),
				Throws.TypeOf<IntegerImporterException>().With.Message.EqualTo(expectedErrorMessage).And.With
					.Property("Row").EqualTo(this.importer.CurrentLineNumber).And.With.Property("Column")
					.EqualTo(columnNumber));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void ShouldThrowWhenGettingTheNullableDecimalValue(bool excelSingleCharOrdinal)
		{
			int columnNumber = GetRandomColumnOrdinal(excelSingleCharOrdinal);
			string expectedErrorMessage = ImporterException.GetExcelStyleErrorMessage(
				this.importer.CurrentLineNumber,
				columnNumber,
				Strings.DecimalImporterErrorAdditionalInfo);
			Assert.That(
				() => this.importer.GetNullableDecimal("1000000000000000.43", columnNumber),
				Throws.TypeOf<DecimalImporterException>().With.Message.EqualTo(expectedErrorMessage).And.With
					.InnerException.Not.Null.And.Property("Row").EqualTo(this.importer.CurrentLineNumber).And.With
					.Property("Column").EqualTo(columnNumber));
		}

		[Test]
		public void ShouldGetTheDecimalValue()
		{
			decimal value = this.importer.GetDecimal("10.25", 1);
			Assert.That(value, Is.EqualTo(10.25d));
		}

		[Test]
		public void ShouldGetTheNullableDecimalValue()
		{
			decimal? value = this.importer.GetNullableDecimal("10.25", 1);
			Assert.That(value, Is.Not.Null);
			Assert.That(value, Is.EqualTo(10.25d));
			value = this.importer.GetNullableDecimal(null, 1);
			Assert.That(value, Is.Null);
		}

		[Test]
		[TestCase("12345", "12345")]
		[TestCase("abcde", "abcde")]
		[TestCase("", null)]
		[TestCase(null, null)]
		public void ShouldGetTheNullableFixedStringValue(string value, string expected)
		{
			string result = this.importer.GetNullableFixedString(value, 1, 100, "TargetPath");
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void ShouldGetTheNullableIntegerValue()
		{
			int? value = this.importer.GetNullableInteger("100", 1);
			Assert.That(value, Is.Not.Null);
			Assert.That(value, Is.EqualTo(100));
			value = this.importer.GetNullableInteger(int.MinValue.ToString(), 1);
			Assert.That(value, Is.Not.Null);
			Assert.That(value, Is.EqualTo(int.MinValue));
			value = this.importer.GetNullableInteger(int.MaxValue.ToString(), 1);
			Assert.That(value, Is.Not.Null);
			Assert.That(value, Is.EqualTo(int.MaxValue));
			value = this.importer.GetNullableInteger(null, 1);
			Assert.That(value, Is.Null);
		}

		[Test]
		[TestCaseSource(typeof(DelimitedFileImporterTestCases), nameof(DelimitedFileImporterTestCases.ReadFileTestCaseSource))]
		public void ShouldValidateTheTestData(string testName, string input, string[] expectedOutput)
		{
			System.Collections.ArrayList outputObj = this.importer.ReadFile(input) as System.Collections.ArrayList;
			string[] outputStringArray = outputObj.Cast<string>().ToArray();
			Console.WriteLine("Test: " + testName);
			CollectionAssert.AreEqual(expectedOutput, outputStringArray);
		}

		private static int GetRandomColumnOrdinal(bool excelSingleCharOrdinal)
		{
			return excelSingleCharOrdinal
				       ? RandomHelper.NextInt32(1, ImporterException.ExcelSingleCharMaxOrdinal - 1)
				       : RandomHelper.NextInt32(ImporterException.ExcelSingleCharMaxOrdinal, 64);
		}
	}
}