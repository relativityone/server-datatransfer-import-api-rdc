// // ----------------------------------------------------------------------------
// <copyright file="ImportApiMetricSinkConfigTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::Monitoring.Sinks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class ImportApiMetricSinkConfigTest
	{
		private IMetricSinkConfig metricSinkConfig;

		[SetUp]
		public void Setup()
		{
			this.metricSinkConfig = new ImportApiMetricSinkConfig();
		}

		[TearDown]
		public void Teardown()
		{
			this.metricSinkConfig = null;
		}

		[Test]
		public void ShouldGetAndSetThrottleTimeout()
		{
			Assert.That(this.metricSinkConfig.ThrottleTimeout, Is.EqualTo(TimeSpan.FromSeconds(AppSettings.Instance.TelemetryMetricsThrottlingSeconds)));
			var expectedValue = TimeSpan.FromSeconds(RandomHelper.NextInt32(1, 1000));
			this.metricSinkConfig.ThrottleTimeout = expectedValue;
			Assert.That(this.metricSinkConfig.ThrottleTimeout, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetSendSumMetrics()
		{
			Assert.That(this.metricSinkConfig.SendSumMetrics, Is.EqualTo(AppSettings.Instance.TelemetrySubmitSumMetrics));
			var expectedValue = RandomHelper.NextBoolean();
			this.metricSinkConfig.SendSumMetrics = expectedValue;
			Assert.That(this.metricSinkConfig.SendSumMetrics, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetSendApmMetrics()
		{
			Assert.That(this.metricSinkConfig.SendApmMetrics, Is.EqualTo(AppSettings.Instance.TelemetrySubmitApmMetrics));
			var expectedValue = RandomHelper.NextBoolean();
			this.metricSinkConfig.SendApmMetrics = expectedValue;
			Assert.That(this.metricSinkConfig.SendApmMetrics, Is.EqualTo(expectedValue));
		}
	}
}