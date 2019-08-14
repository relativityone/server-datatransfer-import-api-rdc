// -----------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensionsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="CollectionExtensions"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents <see cref="CollectionExtensions"/> tests.
	/// </summary>
	[TestFixture]
	public static class CollectionExtensionsTests
	{
		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldGetTheCsvString()
		{
			IEnumerable<string> sequence = new[] { "a", "b", "c", "d" };
			string value1 = sequence.ToCsv();
			Assert.That(value1, Is.EqualTo("a,b,c,d"));
			string value2 = sequence.ToCsv(s => s.ToUpperInvariant());
			Assert.That(value2, Is.EqualTo("A,B,C,D"));
			string value3 = new string[] { }.ToCsv();
			Assert.That(value3, Is.EqualTo(string.Empty));
			Assert.Throws<ArgumentNullException>(() => sequence.ToCsv(null));
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldGetTheDelimitedString()
		{
			IEnumerable<string> sequence = new[] { "a", "b", "c", "d" };
			string value1 = sequence.ToDelimitedString();
			Assert.That(value1, Is.EqualTo("a,b,c,d"));
			string value2 = sequence.ToDelimitedString(";");
			Assert.That(value2, Is.EqualTo("a;b;c;d"));
			string value3 = sequence.ToDelimitedString(";", s => s.ToUpperInvariant());
			Assert.That(value3, Is.EqualTo("A;B;C;D"));
			string value4 = sequence.ToDelimitedString(";", ".");
			Assert.That(value4, Is.EqualTo(".a.;.b.;.c.;.d."));
			string value5 = sequence.ToDelimitedString(";", string.Empty, ".{0}.");
			Assert.That(value5, Is.EqualTo(".a.;.b.;.c.;.d."));
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ToDelimitedStringShouldThrowOnNullForFunction()
		{
			IEnumerable<string> sequence = new[] { "a", "b", "c", "d" };
			Assert.Throws<ArgumentNullException>(
				() => sequence.ToDelimitedString(";", null));
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "We are testing what happens when the sequence is null.")]
		[SuppressMessage("ReSharper", "ExpressionIsAlwaysNull", Justification = "We are testing what happens when the sequence is null.")]
		public static void ToDelimitedStringShouldReturnNullForNullSequence()
		{
			IEnumerable<string> sequence = null;

			var returned = sequence.ToDelimitedString(";", s => s.ToUpperInvariant());
			Assert.IsNull(returned);

			returned = sequence.ToDelimitedString(";", s => throw new NotImplementedException());
			Assert.IsNull(returned);
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ToDelimitedStringShouldReturnEmptyStringIfCollectionIsEmpty()
		{
			IEnumerable<string> sequence = Enumerable.Empty<string>();
			var returned = sequence.ToDelimitedString(";", s => s.ToUpperInvariant());
			Assert.AreEqual(string.Empty, returned);

			IEnumerable<string> sequence2 = Enumerable.Empty<string>();
			var returned2 = sequence2.ToDelimitedString(";", s => throw new NotImplementedException());
			Assert.AreEqual(string.Empty, returned2);
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		[SuppressMessage("ReSharper", "ExpressionIsAlwaysNull", Justification = "We are testing what happens when the sequence is null.")]
		public static void ShouldGetTheIsNullOrEmptyValue()
		{
			IEnumerable<string> sequence = new[] { "1", "2", "3", "4" };
			Assert.That(sequence.IsNullOrEmpty(), Is.False);
			sequence = new string[] { };
			Assert.That(sequence.IsNullOrEmpty(), Is.True);
			sequence = null;
			Assert.That(sequence.IsNullOrEmpty(), Is.True);
		}

		[Test]
		[Category(TestCategories.ExtensionMethods)]
		public static void ShouldGetTheInCollectionValue()
		{
			int[] sequence = { 1, 2, 3, 4 };
			Assert.That(0.In(sequence), Is.False);
			Assert.That(1.In(sequence), Is.True);
			Assert.That(2.In(sequence), Is.True);
			Assert.That(3.In(sequence), Is.True);
			Assert.That(4.In(sequence), Is.True);
			Assert.That(5.In(sequence), Is.False);
		}
	}
}