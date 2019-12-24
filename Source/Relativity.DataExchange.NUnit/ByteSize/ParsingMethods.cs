// <copyright file="ParsingMethods.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit
{
    using System;

    using global::NUnit.Framework;

    using Relativity.DataExchange;

	public class ParsingMethods
	{
		// Base parsing functionality
		[Test]
		public void Parse()
		{
			string val = "1020KB";
			var expected = ByteSize.FromKiloBytes(1020);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void TryParse()
		{
			string val = "1020KB";
			var expected = ByteSize.FromKiloBytes(1020);

			var resultBool = ByteSize.TryParse(val, out var resultByteSize);

			Assert.True(resultBool);
			Assert.AreEqual(expected, resultByteSize);
		}

		[Test]
		public void ParseDecimalMB()
		{
			string val = "100.5MB";
			var expected = ByteSize.FromMegaBytes(100.5);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		// Failure modes
		[Test]
		public void TryParseReturnsFalseOnBadValue()
		{
			string val = "Unexpected Value";

			var resultBool = ByteSize.TryParse(val, out var resultByteSize);

			Assert.False(resultBool);
			Assert.AreEqual(default(ByteSize), resultByteSize);
		}

		[Test]
		public void TryParseReturnsFalseOnMissingMagnitude()
		{
			string val = "1000";

			var resultBool = ByteSize.TryParse(val, out var resultByteSize);

			Assert.False(resultBool);
			Assert.AreEqual(default(ByteSize), resultByteSize);
		}

		[Test]
		public void TryParseReturnsFalseOnMissingValue()
		{
			string val = "KB";

			var resultBool = ByteSize.TryParse(val, out var resultByteSize);

			Assert.False(resultBool);
			Assert.AreEqual(default(ByteSize), resultByteSize);
		}

		[Test]
		public void TryParseWorksWithLotsOfSpaces()
		{
			string val = " 100 KB ";
			var expected = ByteSize.FromKiloBytes(100);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParsePartialBits()
		{
			string val = "10.5b";

			Assert.Throws<FormatException>(() =>
				{
					ByteSize.Parse(val);
				});
		}

		// Parse method throws exceptions
		[Test]
		public void ParseThrowsOnInvalid()
		{
			string badValue = "Unexpected Value";

			Assert.Throws<FormatException>(() =>
				{
					ByteSize.Parse(badValue);
				});
		}

		[Test]
		public void ParseThrowsOnNull()
		{
			Assert.Throws<ArgumentNullException>(() =>
				{
					ByteSize.Parse(null);
				});
		}

		// Various magnitudes
		[Test]
		public void ParseBits()
		{
			string val = "1b";
			var expected = ByteSize.FromBits(1);
			var result = ByteSize.Parse(val);
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParseBytes()
		{
			string val = "1B";
			var expected = ByteSize.FromBytes(1);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParseKB()
		{
			string val = "1020KB";
			var expected = ByteSize.FromKiloBytes(1020);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParseMB()
		{
			string val = "1000MB";
			var expected = ByteSize.FromMegaBytes(1000);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParseGB()
		{
			string val = "805GB";
			var expected = ByteSize.FromGigaBytes(805);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParseTB()
		{
			string val = "100TB";
			var expected = ByteSize.FromTeraBytes(100);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ParsePB()
		{
			string val = "100PB";
			var expected = ByteSize.FromPetaBytes(100);

			var result = ByteSize.Parse(val);

			Assert.AreEqual(expected, result);
		}
	}
}
