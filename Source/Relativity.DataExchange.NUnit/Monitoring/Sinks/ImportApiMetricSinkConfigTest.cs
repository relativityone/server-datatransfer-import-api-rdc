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

	using kCura.WinEDDS.Monitoring;

	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class ImportApiMetricSinkConfigTest
	{
		private IMetricsSinkConfig metricSinkConfig;

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
		public void ShouldGetAndSetSendLiveApmMetrics()
		{
			Assert.That(this.metricSinkConfig.SendLiveApmMetrics, Is.EqualTo(AppSettings.Instance.TelemetrySubmitApmMetrics));
			var expectedValue = RandomHelper.NextBoolean();
			this.metricSinkConfig.SendLiveApmMetrics = expectedValue;
			Assert.That(this.metricSinkConfig.SendLiveApmMetrics, Is.EqualTo(expectedValue));
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
		public void ShouldGetAndSetSendSummaryApmMetrics()
		{
			Assert.That(this.metricSinkConfig.SendSummaryApmMetrics, Is.EqualTo(AppSettings.Instance.TelemetrySubmitApmMetrics));
			var expectedValue = RandomHelper.NextBoolean();
			this.metricSinkConfig.SendSummaryApmMetrics = expectedValue;
			Assert.That(this.metricSinkConfig.SendSummaryApmMetrics, Is.EqualTo(expectedValue));
		}
	}
}