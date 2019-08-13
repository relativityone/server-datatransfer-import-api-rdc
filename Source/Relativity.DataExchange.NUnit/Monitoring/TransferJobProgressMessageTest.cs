// // ----------------------------------------------------------------------------
// <copyright file="TransferJobProgressMessageTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Monitoring;

	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class TransferJobProgressMessageTest
	{
		private TransferJobProgressMessage message;

		[SetUp]
		public void Setup()
		{
			this.message = new TransferJobProgressMessage();
		}

		[Test]
		public void ShouldGetAndSetFileThroughput()
		{
			Assert.That(this.message.FileThroughput, Is.EqualTo(default(double)));
			var expectedValue = RandomHelper.NextDouble(int.MinValue, int.MaxValue);
			this.message.FileThroughput = expectedValue;
			Assert.That(this.message.FileThroughput, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetMetadataThroughput()
		{
			Assert.That(this.message.MetadataThroughput, Is.EqualTo(default(double)));
			var expectedValue = RandomHelper.NextDouble(int.MinValue, int.MaxValue);
			this.message.MetadataThroughput = expectedValue;
			Assert.That(this.message.MetadataThroughput, Is.EqualTo(expectedValue));
		}
	}
}