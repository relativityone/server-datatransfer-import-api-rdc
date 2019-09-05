// // ----------------------------------------------------------------------------
// <copyright file="MetricSinkApmTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
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
		private APMMetric lastLoggedMetric;

		[SetUp]
		public void Setup()
		{
			this.lastLoggedMetric = null;

			var mockApmManager = new Mock<IAPMManager>();
			mockApmManager.Setup(foo => foo.LogCountAsync(It.IsAny<APMMetric>(), It.IsAny<long>())).Callback(
				(APMMetric metric, long count) => this.lastLoggedMetric = metric).Returns(Task.CompletedTask);
			var mockServiceFactory = new Mock<IServiceFactory>();
			mockServiceFactory.Setup(foo => foo.CreateProxy<IAPMManager>()).Returns(mockApmManager.Object);

			this.apmSink = new MetricSinkApm(mockServiceFactory.Object, true);
		}

		[Test]
		public void ShouldLogMetricJobStarted()
		{
			this.apmSink.Log(new MetricJobStarted());
			Assert.NotNull(this.lastLoggedMetric);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_STARTED, this.lastLoggedMetric.Name);
		}

		[Test]
		public void ShouldLogMetricJobProgress()
		{
			this.apmSink.Log(new MetricJobProgress());
			Assert.NotNull(this.lastLoggedMetric);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_PROGRESS, this.lastLoggedMetric.Name);
		}

		[Test]
		public void ShouldLogMetricJobEndReport()
		{
			this.apmSink.Log(new MetricJobEndReport());
			Assert.NotNull(this.lastLoggedMetric);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_END_REPORT, this.lastLoggedMetric.Name);
		}
	}
}