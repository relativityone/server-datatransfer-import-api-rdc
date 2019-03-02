// -----------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="CollectionExtensions"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Relativity.Import.Export;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="StringExtensions"/> tests.
	/// </summary>
	[TestFixture]
    public static class StringExtensionsTests
	{
		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldThrowWhenTheInputIsNull()
		{
			string input = null;
			Assert.Throws<NullReferenceException>(() => input.ToDelimitedFileCellContents("\"", "\n"));
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldThrowWhenTheBoundIsEmpty()
		{
			string input = "real";
			Assert.Throws<ArgumentException>(() => input.ToDelimitedFileCellContents(string.Empty, "\n"));
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldThrowWhenTheBoundIsNull()
		{
			string input = "real";
			Assert.Throws<ArgumentNullException>(() => input.ToDelimitedFileCellContents(null, "\n"));
		}

		[Test]
		[TestCase("\"tok1\" \"tok2\"\r", "\"", "+", "\"\"tok1\"\" \"\"tok2\"\"+")]
		[TestCase("\"tok1\" \"tok2\"\n", "\"", "+", "\"\"tok1\"\" \"\"tok2\"\"+")]
		[TestCase("'tok1' 'tok2'\r\n", "'", "", "\''tok1'' ''tok2''")]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldConvertToDelimitedFileCellContents(string input, string bound, string newlineProxy, string expected)
        {
	        string value = input.ToDelimitedFileCellContents(bound, newlineProxy);
			Assert.That(value, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("\"tok1\" \"tok2\"\n", "\"\"tok1\"\" \"\"tok2\"\"\n")]
		[TestCase("\"tok1\" \"tok2\"\r", "\"\"tok1\"\" \"\"tok2\"\"\n")]
		[TestCase("\"tok1\" \"tok2\"\r\n", "\"\"tok1\"\" \"\"tok2\"\"\n")]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldConvertToCsvCellContents(string input, string expected)
		{
			// The default parameters used by ToCsvCellContents:
			// bound = "\""
			// newlineProxy = "\n"
			string value = input.ToCsvCellContents();
			Assert.That(value, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("ABCDEF123", "ABCDEF123")]
		[TestCase("ABC DEF 123", "ABCDEF123")]
		[TestCase("ABC.DEF.123", "ABCDEF123")]
		[TestCase("ABCDEF123!@#", "ABCDEF123")]
		[TestCase("ABC.DEF.123.!@#", "ABCDEF123")]
		[TestCase(null, "")]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldConvertToSqlFriendlyName(string input, string expected)
		{
			string value = input.ToSqlFriendlyName();
			Assert.That(value, Is.EqualTo(expected));
		}
	}
}