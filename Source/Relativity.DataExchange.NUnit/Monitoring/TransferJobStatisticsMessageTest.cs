// // ----------------------------------------------------------------------------
// <copyright file="TransferJobStatisticsMessageTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Monitoring;

	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class TransferJobStatisticsMessageTest
	{
		private TransferJobStatisticsMessage message;

		[SetUp]
		public void Setup()
		{
			this.message = new TransferJobStatisticsMessage();
		}

		[Test]
		public void ShouldGetAndSetJobSizeInBytes()
		{
			Assert.That(this.message.JobSizeInBytes, Is.EqualTo(default(double)));
			var expectedValue = RandomHelper.NextDouble(int.MinValue, int.MaxValue);
			this.message.JobSizeInBytes = expectedValue;
			Assert.That(this.message.JobSizeInBytes, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetMetadataBytes()
		{
			Assert.That(this.message.MetadataBytes, Is.EqualTo(default(long)));
			var expectedValue = RandomHelper.NextInt64(long.MinValue, long.MaxValue);
			this.message.MetadataBytes = expectedValue;
			Assert.That(this.message.MetadataBytes, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetFileBytes()
		{
			Assert.That(this.message.FileBytes, Is.EqualTo(default(long)));
			var expectedValue = RandomHelper.NextInt64(long.MinValue, long.MaxValue);
			this.message.FileBytes = expectedValue;
			Assert.That(this.message.FileBytes, Is.EqualTo(expectedValue));
		}
	}
}