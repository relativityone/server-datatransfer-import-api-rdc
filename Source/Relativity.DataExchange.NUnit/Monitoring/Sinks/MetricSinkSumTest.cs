// // ----------------------------------------------------------------------------
// <copyright file="MetricSinkSumTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System.Threading.Tasks;
	using global::NUnit.Framework;

	using global::System.Collections.Generic;

	using Monitoring;
	using Monitoring.Sinks;

	using Moq;
	using Relativity.Services.ServiceProxy;
	using Relativity.Telemetry.DataContracts.Shared;
	using Relativity.Telemetry.Services.Metrics;

	[TestFixture]
	public class MetricSinkSumTest
	{
		private MetricSinkSum sumSink;
		private Mock<IServiceFactory> mockServiceFactory;
		private List<MetricRef> loggedMetrics;
		private bool createdProxy;

		[SetUp]
		public void Setup()
		{
			this.loggedMetrics = new List<MetricRef>();
			this.createdProxy = false;

			var mockMetricsManager = new Mock<IMetricsManager>();
			mockMetricsManager
				.Setup(
					foo => foo.LogMetricsAsync(It.IsAny<List<MetricRef>>())).Callback(
					(List<MetricRef> sumMetrics) => this.loggedMetrics.AddRange(sumMetrics)).Returns(Task.CompletedTask);

			this.mockServiceFactory = new Mock<IServiceFactory>();
			this.mockServiceFactory.Setup(foo => foo.CreateProxy<IMetricsManager>()).Returns(mockMetricsManager.Object).Callback(() => this.createdProxy = true);
			this.sumSink = new MetricSinkSum(this.mockServiceFactory.Object, new SumMetricFormatter(), true);
		}

		[Test]
		public void ShouldLogMetricJobStarted()
		{
			var metric = new MetricJobStarted();
			this.sumSink.Log(metric);
			Assert.AreEqual(1, this.loggedMetrics.Count);
		}

		[Test]
		public void ShouldLogMetricJobEndReport()
		{
			var metric = new MetricJobEndReport();
			this.sumSink.Log(metric);
			Assert.AreEqual(6, this.loggedMetrics.Count);
		}

		[Test]
		public void ShouldLogMetricJobProgress()
		{
			var metric = new MetricJobProgress();
			this.sumSink.Log(metric);
			Assert.AreEqual(0, this.loggedMetrics.Count);
		}

		[Test]
		public void ShouldNotCreateProxyIfListIsNull()
		{
			var mockSumMetricFormatter = new Mock<ISumMetricFormatter>();
			mockSumMetricFormatter.Setup(foo => foo.GenerateSumMetrics(It.IsAny<MetricBase>()))
				.Returns((List<MetricRef>)null);
			this.sumSink = new MetricSinkSum(this.mockServiceFactory.Object, mockSumMetricFormatter.Object, true);
			this.sumSink.Log(new MetricJobStarted());
			Assert.False(this.createdProxy);
		}

		[Test]
		public void ShouldNotCreateProxyIfListIsEmpty()
		{
			var mockSumMetricFormatter = new Mock<ISumMetricFormatter>();
			mockSumMetricFormatter.Setup(foo => foo.GenerateSumMetrics(It.IsAny<MetricBase>()))
				.Returns(new List<MetricRef>());
			this.sumSink = new MetricSinkSum(this.mockServiceFactory.Object, mockSumMetricFormatter.Object, true);
			this.sumSink.Log(new MetricJobStarted());
			Assert.False(this.createdProxy);
		}
	}
}