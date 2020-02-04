// // ----------------------------------------------------------------------------
// <copyright file="StatisticsTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	[TestFixture]
	public class StatisticsTest
	{
		private Statistics _statistics;

		[SetUp]
		public void Setup()
		{
			this._statistics = new Statistics();
		}

		[Test]
		public void ShouldNotContainBatchSizeIfEqualsZero()
		{
			// Arrange
			this._statistics.BatchSize = 0;

			// Act
			var actual = this._statistics.ToDictionary();

			// Assert
			Assert.False(actual.Contains(Statistics.CurrentBatchSizeKey));
		}

		[TestCase(500, "500")]
		[TestCase(1_000, "1,000")]
		[TestCase(2_512, "2,512")]
		[TestCase(1_111, "1,111")]
		[TestCase(9_999_999, "9,999,999")]
		[TestCase(3_500, "3,500")]
		public void ShouldFormatBatchSizeProperly(int batchSize, string expected)
		{
			// Arrange
			this._statistics.BatchSize = batchSize;

			// Act
			var actual = this._statistics.ToDictionary()[Statistics.CurrentBatchSizeKey];

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ShouldNotContainSqlRateIfTimeEqualsZero()
		{
			// Arrange
			this._statistics.MassImportDuration = new TimeSpan(0);

			// Act
			var actual = this._statistics.ToDictionary();

			// Assert
			Assert.False(actual.Contains(Statistics.SqlProcessRateKey));
		}

		[TestCase(0, 1, "0 Documents/sec")]
		[TestCase(1, 100_000_000, "0 Documents/sec")]
		[TestCase(2_000, 1, "20,000,000,000 Documents/sec")]
		[TestCase(123, 10_000_000, "123 Documents/sec")]
		[TestCase(100, 100_000_000, "10 Documents/sec")]
		[TestCase(100, 300_000_000, "3 Documents/sec")]
		public void ShouldContainCorrectSqlRate(int documentsCount, long numberOfTicks, string expected)
		{
			// Arrange
			this._statistics.DocumentsCount = documentsCount;
			this._statistics.MassImportDuration = new TimeSpan(numberOfTicks);

			// Act
			var actual = this._statistics.ToDictionary()[Statistics.SqlProcessRateKey];

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ShouldNotContainFileTransferRateIfTimeEqualsZero()
		{
			// Arrange
			this._statistics.FileTransferDuration = new TimeSpan(0);

			// Act
			var actual = this._statistics.ToDictionary();

			// Assert
			Assert.False(actual.Contains(Statistics.FileTransferRateKey));
		}

		[TestCase(0, 2, 1, "0 b/sec")]
		[TestCase(0, 1, 1, "0 b/sec")]
		[TestCase(1, 10_000_000, 0, "1 B/sec")]
		[TestCase(1, 10_000_000, 20_000_000, "1 B/sec")]
		[TestCase(1, 20_000_000, 15_000_000, "2 B/sec")]
		[TestCase(10, 30_000_000, 0, "3.33 B/sec")]
		public void ShouldContainCorrectFileTransferRate(long fileBytes, long transferTicks, long waitTicks, string expected)
		{
			// Arrange
			this._statistics.FileTransferredBytes = fileBytes;
			this._statistics.FileTransferDuration = new TimeSpan(transferTicks);
			this._statistics.FileWaitDuration = new TimeSpan(waitTicks);

			// Act
			var actual = this._statistics.ToDictionary()[Statistics.FileTransferRateKey];

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ShouldNotContainMetadataTransferRateIfTimeEqualsZero()
		{
			// Arrange
			this._statistics.MetadataTransferDuration = new TimeSpan(0);

			// Act
			var actual = this._statistics.ToDictionary();

			// Assert
			Assert.False(actual.Contains(Statistics.MetadataTransferRateKey));
		}

		[TestCase(0, 2, 1, "0 b/sec")]
		[TestCase(0, 1, 1, "0 b/sec")]
		[TestCase(1, 10_000_000, 0, "1 B/sec")]
		[TestCase(1, 10_000_000, 20_000_000, "1 B/sec")]
		[TestCase(1, 20_000_000, 15_000_000, "2 B/sec")]
		[TestCase(10, 30_000_000, 0, "3.33 B/sec")]
		public void ShouldContainCorrectMetadataTransferRate(long metadataBytes, long transferTicks, long waitTicks, string expected)
		{
			// Arrange
			this._statistics.MetadataTransferredBytes = metadataBytes;
			this._statistics.MetadataTransferDuration = new TimeSpan(transferTicks);
			this._statistics.MetadataWaitDuration = new TimeSpan(waitTicks);

			// Act
			var actual = this._statistics.ToDictionary()[Statistics.MetadataTransferRateKey];

			// Assert
			Assert.AreEqual(expected, actual);
		}
	}
}