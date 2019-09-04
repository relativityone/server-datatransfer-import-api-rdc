// // ----------------------------------------------------------------------------
// <copyright file="MetricServiceTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Monitoring;
	using Monitoring.Sinks;

	using Moq;

	using Relativity.Services.ServiceProxy;
	using Relativity.Telemetry.Services.Interface;
	using Relativity.Telemetry.Services.Metrics;

	[TestFixture]
	public class MetricServiceTest
	{
		private IMetricService metricService;
		private System.Collections.Generic.List<APMMetric> loggedApmMetrics;
		private Mock<IServiceFactory> mockServiceFactory;
		private Mock<IMetricSinkConfig> mockMetricSinkConfig;

		[SetUp]
		public void Setup()
		{
			var mockApmManager = new Mock<IAPMManager>();
			mockApmManager.Setup(foo => foo.LogCountAsync(It.IsAny<APMMetric>(), It.IsAny<long>())).Callback(
				(APMMetric metric, long count) => this.loggedApmMetrics.Add(metric)).Returns(Task.CompletedTask);
			this.mockServiceFactory = new Mock<IServiceFactory>();
			this.mockServiceFactory.Setup(foo => foo.CreateProxy<IAPMManager>()).Returns(mockApmManager.Object);

			this.loggedApmMetrics = new System.Collections.Generic.List<APMMetric>();
			this.mockMetricSinkConfig = new Mock<IMetricSinkConfig>();
			this.mockMetricSinkConfig.Setup(foo => foo.ThrottleTimeout).Returns(TimeSpan.FromSeconds(30));
			this.mockMetricSinkConfig.Setup(foo => foo.SendSumMetrics).Returns(true);
			this.mockMetricSinkConfig.Setup(foo => foo.SendApmMetrics).Returns(true);
		}

		[Test]
		public void ShouldLogApmMetricsWhenEnabled()
		{
			// Arrange
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object, this.mockServiceFactory.Object);

			// Act
			this.metricService.Log(new MetricJobEndReport());
			this.metricService.Log(new MetricJobStarted());
			this.metricService.Log(new MetricJobProgress());

			// Assert
			Assert.AreEqual(3, this.loggedApmMetrics.Count);
		}

		[Test]
		public void ShouldNotLogApmMetricsWhenDisabled()
		{
			// Arrange
			this.mockMetricSinkConfig.Setup(foo => foo.SendApmMetrics).Returns(false);
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object, this.mockServiceFactory.Object);

			// Act
			this.metricService.Log(new MetricJobEndReport());
			this.metricService.Log(new MetricJobStarted());
			this.metricService.Log(new MetricJobProgress());

			// Assert
			Assert.AreEqual(0, this.loggedApmMetrics.Count);
		}

		[Test]
		public void ShouldUpdateSinksConfiguration()
		{
			// Arrange
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object, this.mockServiceFactory.Object);
			this.mockMetricSinkConfig.Setup(foo => foo.SendApmMetrics).Returns(false);
			this.mockMetricSinkConfig.Setup(foo => foo.SendSumMetrics).Returns(false);
			this.metricService.MetricSinkConfig = this.mockMetricSinkConfig.Object;

			// Act
			this.metricService.Log(new MetricJobEndReport());
			this.metricService.Log(new MetricJobStarted());
			this.metricService.Log(new MetricJobProgress());

			// Assert
			Assert.AreEqual(0, this.loggedApmMetrics.Count);
		}
	}
}