// // ----------------------------------------------------------------------------
// <copyright file="MetricSinkApmTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System.Linq;
	using System.Threading.Tasks;
	using global::Monitoring.Sinks;
	using global::NUnit.Framework;
	using Monitoring;
	using Moq;
	using Relativity.Services.ServiceProxy;
	using Relativity.Telemetry.Services.Interface;
	using Relativity.Telemetry.Services.Metrics;

	[TestFixture]
	public class MetricSinkApmTest
	{
		private MetricSinkApm apmSink;
		private System.Collections.Generic.List<APMMetric> loggedMetrics;

		[SetUp]
		public void Setup()
		{
			var mockApmManager = new Mock<IAPMManager>();
			mockApmManager.Setup(foo => foo.LogCountAsync(It.IsAny<APMMetric>(), It.IsAny<long>())).Callback(
				(APMMetric metric, long count) => this.loggedMetrics.Add(metric)).Returns(Task.CompletedTask);
			var mockServiceFactory = new Mock<IServiceFactory>();
			mockServiceFactory.Setup(foo => foo.CreateProxy<IAPMManager>()).Returns(mockApmManager.Object);

			this.loggedMetrics = new System.Collections.Generic.List<APMMetric>();
			this.apmSink = new MetricSinkApm(mockServiceFactory.Object, true);
		}

		[Test]
		public void ShouldLogMetricJobStarted()
		{
			var metric = new MetricJobStarted();
			this.apmSink.Log(metric);
			Assert.AreEqual(1, this.loggedMetrics.Count);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_STARTED, this.loggedMetrics.Last().Name);
		}

		[Test]
		public void ShouldLogMetricJobProgress()
		{
			var metric = new MetricJobProgress();
			this.apmSink.Log(metric);
			Assert.AreEqual(1, this.loggedMetrics.Count);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_PROGRESS, this.loggedMetrics.Last().Name);
		}

		[Test]
		public void ShouldLogMetricJobEndReport()
		{
			var metric = new MetricJobEndReport();
			this.apmSink.Log(metric);
			Assert.AreEqual(1, this.loggedMetrics.Count);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_END_REPORT, this.loggedMetrics.Last().Name);
		}
	}
}