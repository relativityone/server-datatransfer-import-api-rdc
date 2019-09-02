// // ----------------------------------------------------------------------------
// <copyright file="MetricSinkManagerTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using global::Monitoring.Sinks;
	using global::NUnit.Framework;
	using kCura.WinEDDS.Monitoring;
	using Monitoring;
	using Moq;
	using Relativity.DataTransfer.MessageService;
	using Relativity.DataTransfer.MessageService.MetricsManager.APM;

	[TestFixture]
	public class MetricSinkManagerTest
	{
		private const int _MAX_TEST_DELAY = 1_000;

		private Mock<IMetricsSinkConfig> mockMetricsSinkConfig;
		private IMetricSinkManager metricSinkManager;
		private long logApmDoubleCalls = 0;

		[SetUp]
		public void Setup()
		{
			// reset counters
			this.logApmDoubleCalls = 0;

			// calling Log methods increments counters
			var mockApmMetricsManager = new Mock<IMetricsManager>();
			mockApmMetricsManager.Setup(
				foo => foo.LogDouble(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<IMetricMetadata>())).Callback(() => this.logApmDoubleCalls++);

			// MetricsManagerFactory creates mockMetricsManager
			var mockMetricsManagerFactory = new Mock<IMetricsManagerFactory>();
			mockMetricsManagerFactory.Setup(foo => foo.CreateAPMKeplerManager(null))
				.Returns(mockApmMetricsManager.Object);

			this.mockMetricsSinkConfig = new Mock<IMetricsSinkConfig>();
			this.mockMetricsSinkConfig.SetupGet(foo => foo.ThrottleTimeout).Returns(TimeSpan.FromSeconds(30));
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSummaryApmMetrics).Returns(true);
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendLiveApmMetrics).Returns(true);
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSumMetrics).Returns(true);

			this.metricSinkManager = new MetricSinkManager(mockMetricsManagerFactory.Object, null);
		}

		[Test]
		public void ShouldLogMetricJobStarted()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new MetricJobStarted());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogMetricJobProgress()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new MetricJobProgress());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogMetricJobEndReport()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new MetricJobEndReport());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldNotSendSummaryApmMetricsIfSummaryTurnedOff()
		{
			// turn off sending summary apm metrics
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSummaryApmMetrics).Returns(false);

			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new MetricJobEndReport());
			messageService.Send(new MetricJobStarted());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldSendSummaryApmMetricsIfLiveTurnedOff()
		{
			// turn off sending live apm metrics
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendLiveApmMetrics).Returns(false);

			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new MetricJobEndReport());
			messageService.Send(new MetricJobStarted());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(2).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldSendLiveApmMetricsIfSummaryTurnedOff()
		{
			// turn off sending summary apm metrics
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSummaryApmMetrics).Returns(false);

			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new MetricJobProgress());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldNotSendLiveApmMetricsIfLiveTurnedOff()
		{
			// turn off sending live apm metrics
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendLiveApmMetrics).Returns(false);

			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new MetricJobProgress());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldLogMetricJobProgressOnlyOnceInGivenTimeout()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			for (int i = 0; i < 50; i++)
			{
				messageService.Send(new MetricJobProgress());
			}

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(_MAX_TEST_DELAY));
		}
	}
}