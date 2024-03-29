﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="CollectionExtensions"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;

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

		[TestCase(null, "")]
		[TestCase("", "")]
		[TestCase(@"https://relativity.one", "https://relativity.one/")]
		[TestCase(@"https://relativity.one/", "https://relativity.one/")]
		public static void ShouldAppendTheTrailingSlashToTheUrl(string value, string expected)
		{
			string actual = value.AppendTrailingSlashToUrl();
			Assert.That(actual, Is.EqualTo(expected));
		}

		[TestCase(null, "")]
		[TestCase("", "")]
		[TestCase(@"/relativity.one", "relativity.one")]
		[TestCase(@"\relativity.one", "relativity.one")]
		public static void ShouldTrimTheLeadingSlashFromTheUrl(string value, string expected)
		{
			string actual = value.TrimLeadingSlashFromUrl();
			Assert.That(actual, Is.EqualTo(expected));
		}

		[TestCase(null, "")]
		[TestCase("", "")]
		[TestCase(@"https://relativity.one", "https://relativity.one")]
		[TestCase(@"https://relativity.one/", "https://relativity.one")]
		[TestCase(@"https://relativity.one\\", "https://relativity.one")]
		public static void ShouldTrimTheTrailingSlashFromTheUrl(string value, string expected)
		{
			string actual = value.TrimTrailingSlashFromUrl();
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(null, null, "")]
		[TestCase(null, "", "")]
		[TestCase("", null, "")]
		[TestCase("", "", "")]
		[TestCase(@"https://relativity.one", "Service.API/Module/GetValue", "https://relativity.one/Service.API/Module/GetValue")]
		[TestCase(@"https://relativity.one", "/Service.API/Module/GetValue", "https://relativity.one/Service.API/Module/GetValue")]
		[TestCase(@"https://relativity.one/", "/Service.API/Module/GetValue", "https://relativity.one/Service.API/Module/GetValue")]
		[TestCase(@"https://relativity.one", "", "https://relativity.one/")]
		[TestCase(@"https://relativity.one/", "", "https://relativity.one/")]
		public static void ShouldCombineTheUrls(string url1, string url2, string expected)
		{
			string actual = url1.CombineUrls(url2);
			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}