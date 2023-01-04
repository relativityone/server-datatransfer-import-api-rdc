// // ----------------------------------------------------------------------------
// <copyright file="ByteArrayConverterTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using global::NUnit.Framework;
	using Relativity.DataExchange.Media;

	[TestFixture]
	public class ByteArrayConverterTest
	{
		private ByteArrayConverter byteArrayConverter;

		[SetUp]
		public void Setup()
		{
			this.byteArrayConverter = new ByteArrayConverter();
		}

		[Test]
		public void ShouldThrowWhenByteArrayIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => this.byteArrayConverter.ToInt64(null, 0, 0, ByteOrdering.BigEndian));
		}

		[Test]
		[TestCase(-4)]
		[TestCase(0)]
		[TestCase(8)]
		[TestCase(28)]
		public void ShouldThrowWhenLengthOutOfRange(int length)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => this.byteArrayConverter.ToInt64(new byte[9], 0, length, ByteOrdering.BigEndian));
		}

		[Test]
		[TestCase(-4, 3)]
		[TestCase(0, 6)]
		public void ShouldThrowWhenStartOutOfRange(int start, int length)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => this.byteArrayConverter.ToInt64(new byte[5], start, length, ByteOrdering.BigEndian));
		}

		[Test]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 7, 2, 11612)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 6, 2, 23564)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 5, 2, 3155)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 1, 2, 25910)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 3, 2, 65476)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 0, 4, 3294967296)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 3, 4, 206831556)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 5, 4, 761007187)]
		[TestCase(new byte[] { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 }, 5, 4, 4057526311)]
		[TestCase(new byte[] { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 }, 3, 4, 4029091968)]
		[TestCase(new byte[] { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 }, 0, 4, 2147483663)]
		public void ShouldConvertCorrectlyForLittleEndian(byte[] byteArray, int start, int length, long expected)
		{
			var actual = this.byteArrayConverter.ToInt64(byteArray, start, length, ByteOrdering.LittleEndian);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 7, 2, 23597)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 6, 2, 3164)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 5, 2, 21260)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 1, 2, 13925)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 3, 2, 50431)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 0, 4, 3564996)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 3, 4, 3305067276)]
		[TestCase(new byte[] { 0, 54, 101, 196, 255, 83, 12, 92, 45 }, 5, 4, 1393318957)]
		[TestCase(new byte[] { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 }, 5, 4, 670095601)]
		[TestCase(new byte[] { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 }, 3, 4, 2148542448)]
		[TestCase(new byte[] { 15, 0, 0, 128, 16, 39, 240, 216, 241, 255, 127 }, 0, 4, 251658368)]
		public void ShouldConvertCorrectlyForBigEndian(byte[] byteArray, int start, int length, long expected)
		{
			var actual = this.byteArrayConverter.ToInt64(byteArray, start, length, ByteOrdering.BigEndian);
			Assert.AreEqual(expected, actual);
		}
	}
}