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

			var mockServiceFactory = new Mock<IServiceFactory>();
			mockServiceFactory.Setup(foo => foo.CreateProxy<IMetricsManager>()).Returns(mockMetricsManager.Object).Callback(() => this.createdProxy = true);
			this.sumSink = new MetricSinkSum(mockServiceFactory.Object, true);
		}

		[Test]
		public void ShouldLogMetricJobStarted()
		{
			var metric = new MetricJobStarted();
			this.sumSink.Log(metric);
			Assert.AreEqual(metric.GenerateSumMetrics().Count, this.loggedMetrics.Count);
		}

		[Test]
		public void ShouldLogMetricJobEndReport()
		{
			var metric = new MetricJobEndReport();
			this.sumSink.Log(metric);
			Assert.AreEqual(metric.GenerateSumMetrics().Count, this.loggedMetrics.Count);
		}

		[Test]
		public void ShouldNotCreateProxyIfListIsNull()
		{
			var metric = new Mock<MetricBase>();
			metric.Setup(foo => foo.GenerateSumMetrics()).Returns((List<MetricRef>)null);
			this.sumSink.Log(metric.Object);
			Assert.False(this.createdProxy);
		}

		[Test]
		public void ShouldNotCreateProxyIfListIsEmpty()
		{
			var metric = new Mock<MetricBase>();
			metric.Setup(foo => foo.GenerateSumMetrics()).Returns(new List<MetricRef>());
			this.sumSink.Log(metric.Object);
			Assert.False(this.createdProxy);
		}
	}
}