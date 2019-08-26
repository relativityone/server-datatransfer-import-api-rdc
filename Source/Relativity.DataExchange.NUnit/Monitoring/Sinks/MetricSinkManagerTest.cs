// // ----------------------------------------------------------------------------
// <copyright file="MetricSinkManagerTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Monitoring;

	using Monitoring.Sinks;

	using Moq;

	using Relativity.DataTransfer.MessageService;
	using Relativity.DataTransfer.MessageService.MetricsManager.APM;

	[TestFixture]
	public class MetricSinkManagerTest
	{
		private const int _MAX_TEST_DELAY = 1_000;

		private Mock<IMetricsSinkConfig> mockMetricsSinkConfig;
		private IMetricSinkManager metricSinkManager;
		private long logSumDoubleCalls = 0;
		private long logSumCountCalls = 0;
		private long logApmDoubleCalls = 0;

		[SetUp]
		public void Setup()
		{
			// reset counters
			this.logSumDoubleCalls = 0;
			this.logSumCountCalls = 0;
			this.logApmDoubleCalls = 0;

			// calling Log methods increments counters
			var mockSumMetricsManager = new Mock<IMetricsManager>();
			mockSumMetricsManager.Setup(
				foo => foo.LogCount(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<IMetricMetadata>())).Callback(() => this.logSumCountCalls++);
			mockSumMetricsManager.Setup(
				foo => foo.LogDouble(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<IMetricMetadata>())).Callback(() => this.logSumDoubleCalls++);

			var mockApmMetricsManager = new Mock<IMetricsManager>();
			mockApmMetricsManager.Setup(
				foo => foo.LogDouble(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<IMetricMetadata>())).Callback(() => this.logApmDoubleCalls++);

			// MetricsManagerFactory creates mockMetricsManager
			var mockMetricsManagerFactory = new Mock<IMetricsManagerFactory>();
			mockMetricsManagerFactory.Setup(foo => foo.CreateAPMKeplerManager(null))
				.Returns(mockApmMetricsManager.Object);
			mockMetricsManagerFactory.Setup(foo => foo.CreateSUMKeplerManager(null))
				.Returns(mockSumMetricsManager.Object);

			this.mockMetricsSinkConfig = new Mock<IMetricsSinkConfig>();
			this.mockMetricsSinkConfig.SetupGet(foo => foo.ThrottleTimeout).Returns(TimeSpan.FromSeconds(30));
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSummaryApmMetrics).Returns(true);
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendLiveApmMetrics).Returns(true);
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSumMetrics).Returns(true);

			this.metricSinkManager = new MetricSinkManager(mockMetricsManagerFactory.Object, null);
		}

		[Test]
		public void ShouldLogTransferJobStartedMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobStartedMessage());

			Assert.That(() => this.logSumCountCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogTransferJobCompletedMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobCompletedMessage());

			Assert.That(() => this.logSumCountCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogTransferJobFailedMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobFailedMessage());

			Assert.That(() => this.logSumCountCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogTransferJobTotalRecordsCountMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobTotalRecordsCountMessage());

			Assert.That(() => this.logSumDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogTransferJobThroughputMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobThroughputMessage());

			Assert.That(() => this.logSumDoubleCalls, Is.EqualTo(2).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogTransferJobCompletedRecordsCountMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobCompletedRecordsCountMessage());

			Assert.That(() => this.logSumDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogTransferJobStatisticsMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobStatisticsMessage());

			// TransferJobStatisticsMessage is logging 2 metrics in OnMessage methods <see cref="JobSumEndOfLifeSink"/> and <see cref="JobApmEndOfLifeSink"/>
			Assert.That(() => this.logSumDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldLogTransferJobProgressMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobProgressMessage());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(delayInMilliseconds: _MAX_TEST_DELAY, pollingInterval: 100));
		}

		[Test]
		public void ShouldNotLogUnregisteredMessage()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobMessageBase());

			Assert.That(() => this.logSumDoubleCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
			Assert.That(() => this.logSumCountCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldNotSendSumMetricsIfTurnedOff()
		{
			// turn off sending sum metrics
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSumMetrics).Returns(false);

			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobStartedMessage());
			messageService.Send(new TransferJobCompletedMessage());
			messageService.Send(new TransferJobFailedMessage());
			messageService.Send(new TransferJobThroughputMessage());
			messageService.Send(new TransferJobTotalRecordsCountMessage());
			messageService.Send(new TransferJobCompletedRecordsCountMessage());
			messageService.Send(new TransferJobStatisticsMessage());

			Assert.That(() => this.logSumDoubleCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldNotSendSummaryApmMetricsIfTurnedOff()
		{
			// turn off sending summary apm metrics
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendSummaryApmMetrics).Returns(false);

			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobStatisticsMessage());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldNotSendLiveApmMetricsIfTurnedOff()
		{
			// turn off sending live apm metrics
			this.mockMetricsSinkConfig.SetupGet(foo => foo.SendLiveApmMetrics).Returns(false);

			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			messageService.Send(new TransferJobProgressMessage());

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(0).After(_MAX_TEST_DELAY));
		}

		[Test]
		public void ShouldLogTransferJobProgressMessageOnlyOnceInGivenTimeout()
		{
			var messageService = this.metricSinkManager.SetupMessageService(this.mockMetricsSinkConfig.Object);
			for (int i = 0; i < 50; i++)
			{
				messageService.Send(new TransferJobProgressMessage());
			}

			Assert.That(() => this.logApmDoubleCalls, Is.EqualTo(1).After(_MAX_TEST_DELAY));
		}
	}
}