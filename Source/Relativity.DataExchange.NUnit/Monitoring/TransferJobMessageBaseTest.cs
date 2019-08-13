// // ----------------------------------------------------------------------------
// <copyright file="TransferJobMessageBaseTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Monitoring;

	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class TransferJobMessageBaseTest
	{
		private TransferJobMessageBase message;

		[SetUp]
		public void Setup()
		{
			this.message = new TransferJobMessageBase();
		}

		[Test]
		public void ShouldGetAndSetJobType()
		{
			Assert.That(this.message.JobType, Is.Null);
			var expectedValue = RandomHelper.NextString(10, 20);
			this.message.JobType = expectedValue;
			Assert.That(this.message.JobType, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTransferMode()
		{
			Assert.That(this.message.TransferMode, Is.Null);
			var expectedValue = RandomHelper.NextString(10, 20);
			this.message.TransferMode = expectedValue;
			Assert.That(this.message.TransferMode, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetApplicationName()
		{
			Assert.That(this.message.ApplicationName, Is.EqualTo(AppSettings.Instance.ApplicationName));
			var expectedValue = RandomHelper.NextString(10, 20);
			this.message.ApplicationName = expectedValue;
			Assert.That(this.message.ApplicationName, Is.EqualTo(expectedValue));
		}
	}
}