// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiTotalsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="TapiTotals"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	/// <summary>
	/// Represents <see cref="TapiTotals"/> tests.
	/// </summary>
	[TestFixture]
	public class TapiTotalsTests
	{
		private long testTotalCompletedFileTransfers;
		private long testTotalFileTransferRequests;
		private long testTotalSuccessfulFileTransfers;
		private TapiTotals totals;

		[SetUp]
		public void Setup()
		{
			this.testTotalCompletedFileTransfers = RandomHelper.NextInt64(1, int.MaxValue);
			this.testTotalFileTransferRequests = RandomHelper.NextInt64(1, int.MaxValue);
			this.testTotalSuccessfulFileTransfers = RandomHelper.NextInt64(1, int.MaxValue);
			this.totals = new TapiTotals(
				this.testTotalCompletedFileTransfers,
				this.testTotalFileTransferRequests,
				this.testTotalSuccessfulFileTransfers);
			Assert.That(this.totals.TotalCompletedFileTransfers, Is.EqualTo(this.testTotalCompletedFileTransfers));
			Assert.That(this.totals.TotalFileTransferRequests, Is.EqualTo(this.testTotalFileTransferRequests));
			Assert.That(this.totals.TotalSuccessfulFileTransfers, Is.EqualTo(this.testTotalSuccessfulFileTransfers));
		}

		[Test]
		public void ShouldDeepCopyTheTotals()
		{
			TapiTotals copy = this.totals.DeepCopy();
			Assert.That(copy, Is.Not.SameAs(this.totals));
			Assert.That(copy.TotalCompletedFileTransfers, Is.EqualTo(this.testTotalCompletedFileTransfers));
			Assert.That(copy.TotalFileTransferRequests, Is.EqualTo(this.testTotalFileTransferRequests));
			Assert.That(copy.TotalSuccessfulFileTransfers, Is.EqualTo(this.testTotalSuccessfulFileTransfers));
		}

		[Test]
		public void ShouldClearTheTotals()
		{
			Assert.That(this.totals.TotalCompletedFileTransfers, Is.Not.Zero);
			Assert.That(this.totals.TotalFileTransferRequests, Is.Not.Zero);
			Assert.That(this.totals.TotalSuccessfulFileTransfers, Is.Not.Zero);
			this.totals.Clear();
			Assert.That(this.totals.TotalCompletedFileTransfers, Is.Zero);
			Assert.That(this.totals.TotalFileTransferRequests, Is.Zero);
			Assert.That(this.totals.TotalSuccessfulFileTransfers, Is.Zero);
		}

		[Test]
		public void ShouldIncrementTheTotals()
		{
			const int Iterations = 10;
			for (int i = 0; i < Iterations; i++)
			{
				this.totals.IncrementTotalFileTransferRequests();
				this.totals.IncrementTotalCompletedFileTransfers();
				this.totals.IncrementTotalSuccessfulFileTransfers();
				Assert.That(this.totals.TotalCompletedFileTransfers, Is.EqualTo(this.testTotalCompletedFileTransfers + i + 1));
				Assert.That(this.totals.TotalFileTransferRequests, Is.EqualTo(this.testTotalFileTransferRequests + i + 1));
				Assert.That(this.totals.TotalSuccessfulFileTransfers, Is.EqualTo(this.testTotalSuccessfulFileTransfers + i + 1));
			}
		}
	}
}