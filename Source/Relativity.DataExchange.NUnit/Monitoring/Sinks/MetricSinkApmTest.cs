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

	using Relativity.DataExchange.NUnit.Mocks;
	using Relativity.DataExchange.Service;
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
			// Arrange
			var mockApmManager = new Mock<IAPMManager>();
			mockApmManager.Setup(foo => foo.LogCountAsync(It.IsAny<APMMetric>(), It.IsAny<long>())).Callback(
				(APMMetric metric, long count) => this.lastLoggedMetric = metric).Returns(Task.CompletedTask);
			var mockServiceFactory = new Mock<IServiceProxyFactory>();
			mockServiceFactory.Setup(foo => foo.CreateProxyInstance<IAPMManager>()).Returns(mockApmManager.Object);
			this.apmSink = new MetricSinkApm(new KeplerProxyMock(mockServiceFactory.Object), true);
			this.lastLoggedMetric = null;
		}

		[Test]
		public void ShouldLogMetricJobStarted()
		{
			// Act
			this.apmSink.Log(new MetricJobStarted());

			// Assert
			Assert.NotNull(this.lastLoggedMetric);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_STARTED, this.lastLoggedMetric.Name);
		}

		[Test]
		public void ShouldLogMetricJobProgress()
		{
			// Act
			this.apmSink.Log(new MetricJobProgress());

			// Assert
			Assert.NotNull(this.lastLoggedMetric);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_PROGRESS, this.lastLoggedMetric.Name);
		}

		[Test]
		public void ShouldLogMetricJobEndReport()
		{
			// Act
			this.apmSink.Log(new MetricJobEndReport());

			// Assert
			Assert.NotNull(this.lastLoggedMetric);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_JOB_END_REPORT, this.lastLoggedMetric.Name);
		}

		[Test]
		public void ShouldLogMetricAuthenticationType()
		{
			// Act
			this.apmSink.Log(new MetricAuthenticationType());

			// Assert
			Assert.NotNull(this.lastLoggedMetric);
			Assert.AreEqual(TelemetryConstants.BucketName.METRIC_AUTHENTICATION_TYPE, this.lastLoggedMetric.Name);
		}
	}
}