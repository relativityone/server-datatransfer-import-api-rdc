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
	using global::System.Collections.Generic;

	using Monitoring;
	using Monitoring.Sinks;

	using Moq;

	using Relativity.DataExchange.NUnit.Mocks;
	using Relativity.DataExchange.Service;
	using Relativity.Telemetry.DataContracts.Shared;
	using Relativity.Telemetry.Services.Interface;
	using Relativity.Telemetry.Services.Metrics;

	[TestFixture]
	public class MetricServiceTest
	{
		private long loggedApmMetricsCount;
		private long loggedSumMetricsCount;

		private IMetricService metricService;
		private Mock<IServiceProxyFactory> mockServiceFactory;
		private Mock<IMetricSinkConfig> mockMetricSinkConfig;

		[SetUp]
		public void Setup()
		{
			this.loggedApmMetricsCount = 0;
			this.loggedSumMetricsCount = 0;

			var mockApmManager = new Mock<IAPMManager>();
			mockApmManager.Setup(foo => foo.LogCountAsync(It.IsAny<APMMetric>(), It.IsAny<long>())).Callback(
				(APMMetric metric, long count) => this.loggedApmMetricsCount++).Returns(Task.CompletedTask);

			var mockMetricsManager = new Mock<IMetricsManager>();
			mockMetricsManager
				.Setup(
					foo => foo.LogMetricsAsync(It.IsAny<List<MetricRef>>())).Callback(() => this.loggedSumMetricsCount++).Returns(Task.CompletedTask);

			this.mockServiceFactory = new Mock<IServiceProxyFactory>();
			this.mockServiceFactory.Setup(foo => foo.CreateProxyInstance<IAPMManager>()).Returns(mockApmManager.Object);

			this.mockServiceFactory.Setup(foo => foo.CreateProxyInstance<IMetricsManager>()).Returns(mockMetricsManager.Object);

			this.mockMetricSinkConfig = new Mock<IMetricSinkConfig>();
			this.mockMetricSinkConfig.Setup(foo => foo.ThrottleTimeout).Returns(TimeSpan.FromSeconds(30));
			this.mockMetricSinkConfig.Setup(foo => foo.SendSumMetrics).Returns(true);
			this.mockMetricSinkConfig.Setup(foo => foo.SendApmMetrics).Returns(true);
		}

		[Test]
		public void ShouldLogApmMetricsWhenEnabled()
		{
			// Arrange
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object, new KeplerProxyMock(this.mockServiceFactory.Object));

			// Act
			this.metricService.Log(new MetricJobEndReport());
			this.metricService.Log(new MetricJobStarted());
			this.metricService.Log(new MetricJobProgress());

			// Assert
			Assert.AreEqual(3, this.loggedApmMetricsCount);
		}

		[Test]
		public void ShouldLogSumMetricsWhenEnabled()
		{
			// Arrange
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object, new KeplerProxyMock(this.mockServiceFactory.Object));

			// Act
			this.metricService.Log(new MetricJobEndReport());
			this.metricService.Log(new MetricJobStarted());

			// Assert
			Assert.AreEqual(2, this.loggedSumMetricsCount);
		}

		[Test]
		public void ShouldNotLogApmMetricsWhenDisabled()
		{
			// Arrange
			this.mockMetricSinkConfig.Setup(foo => foo.SendApmMetrics).Returns(false);
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object, new KeplerProxyMock(this.mockServiceFactory.Object));

			// Act
			this.metricService.Log(new MetricJobEndReport());
			this.metricService.Log(new MetricJobStarted());
			this.metricService.Log(new MetricJobProgress());

			// Assert
			Assert.AreEqual(0, this.loggedApmMetricsCount);
		}

		[Test]
		public void ShouldNotLogSumMetricsWhenDisabled()
		{
			// Arrange
			this.mockMetricSinkConfig.Setup(foo => foo.SendSumMetrics).Returns(false);
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object, new KeplerProxyMock(this.mockServiceFactory.Object));

			// Act
			this.metricService.Log(new MetricJobEndReport());
			this.metricService.Log(new MetricJobStarted());

			// Assert
			Assert.AreEqual(0, this.loggedSumMetricsCount);
		}

		[Test]
		public void ShouldNotFailWhenLogWithoutServiceFactory()
		{
			// Arrange
			this.metricService = new MetricService(this.mockMetricSinkConfig.Object);

			Assert.DoesNotThrow(() => this.metricService.Log(new MetricJobEndReport()));
			Assert.DoesNotThrow(() => this.metricService.Log(new MetricJobStarted()));
			Assert.DoesNotThrow(() => this.metricService.Log(new MetricJobProgress()));
		}
	}
}