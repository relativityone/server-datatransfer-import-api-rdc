// -----------------------------------------------------------------------------------------------------
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

	using global::NUnit.Framework;

	using Relativity.Import.Export.Io;

	/// <summary>
	/// Represents <see cref="DelimitedFileImporter"/> tests.
	/// </summary>
	[TestFixture]
	public class DelimitedFileImporterTests
	{
		private DelimitedFileImporter importer;

		[SetUp]
		public void Setup()
		{
			this.importer = new MockDelimitedFileImport();
		}

		[TearDown]
		public void TearDown()
		{
			this.importer.Close();
		}

		[Test]
		public void ShouldGetTheDecimalValue()
		{
			decimal value = this.importer.GetDecimal("10.25", 1);
			Assert.That(value, Is.EqualTo(10.25d));
		}

		[Test]
		public void ShouldThrowWhenGettingTheDecimalValue()
		{
			Assert.Throws<DecimalImporterException>(() => this.importer.GetDecimal(null, 1));
			Assert.Throws<DecimalImporterException>(() => this.importer.GetDecimal("1000000000000000.43", 1));
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
		public void ShouldThrowWhenTheNullableFixedStringExceedsTheMaxLength()
		{
			StringImporterException exception = Assert.Throws<StringImporterException>(
				() =>
					{
						this.importer.GetNullableFixedString("abcde", 1, 3, "TargetPath");
					});
			string additionalInfoMessage = StringImporterException.GetAdditionalInfoMessage(5, 3, "TargetPath");
			string errorMessage = ImporterException.GetErrorMessage(
				this.importer.CurrentLineNumber,
				"TargetPath",
				additionalInfoMessage);
			Assert.That(exception.Message, Is.EqualTo(errorMessage));
		}

		[Test]
		public void ShouldThrowWhenGettingTheNullableDecimalValue()
		{
			Assert.Throws<DecimalImporterException>(() => this.importer.GetNullableDecimal("1000000000000000.43", 1));
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
	}
}