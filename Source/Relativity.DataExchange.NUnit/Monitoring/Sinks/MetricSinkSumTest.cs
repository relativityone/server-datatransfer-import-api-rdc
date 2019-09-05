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

		[SetUp]
		public void Setup()
		{
			this.loggedMetrics = new List<MetricRef>();

			var mockMetricsManager = new Mock<IMetricsManager>();
			mockMetricsManager
				.Setup(
					foo => foo.LogMetricsAsync(It.IsAny<List<MetricRef>>())).Callback(
					(List<MetricRef> sumMetrics) => this.loggedMetrics.AddRange(sumMetrics)).Returns(Task.CompletedTask);

			var mockServiceFactory = new Mock<IServiceFactory>();
			mockServiceFactory.Setup(foo => foo.CreateProxy<IMetricsManager>()).Returns(mockMetricsManager.Object);
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
		public void ShouldLogMetricJobProgress()
		{
			var metric = new MetricJobProgress();
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
	}
}