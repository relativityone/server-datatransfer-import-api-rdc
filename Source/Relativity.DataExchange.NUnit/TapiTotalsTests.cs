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
		[Test]
		public void ShouldDeepCopyTheTotals()
		{
			long testTotalCompletedFileTransfers = RandomHelper.NextInt64(1, int.MaxValue);
			long testTotalFileTransferRequests = RandomHelper.NextInt64(1, int.MaxValue);
			long testTotalSuccessfulFileTransfers = RandomHelper.NextInt64(1, int.MaxValue);
			TapiTotals totals = new TapiTotals(
				testTotalCompletedFileTransfers,
				testTotalFileTransferRequests,
				testTotalSuccessfulFileTransfers);

			Assert.That(totals.TotalCompletedFileTransfers, Is.EqualTo(testTotalCompletedFileTransfers));
			Assert.That(totals.TotalFileTransferRequests, Is.EqualTo(testTotalFileTransferRequests));
			Assert.That(totals.TotalSuccessfulFileTransfers, Is.EqualTo(testTotalSuccessfulFileTransfers));
			TapiTotals copy = totals.DeepCopy();
			Assert.That(copy, Is.Not.SameAs(totals));
			Assert.That(copy.TotalCompletedFileTransfers, Is.EqualTo(testTotalCompletedFileTransfers));
			Assert.That(copy.TotalFileTransferRequests, Is.EqualTo(testTotalFileTransferRequests));
			Assert.That(copy.TotalSuccessfulFileTransfers, Is.EqualTo(testTotalSuccessfulFileTransfers));
		}
	}
}