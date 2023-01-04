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

	using Relativity.DataExchange.NUnit.Mocks;
	using Relativity.DataExchange.Service;
	using Relativity.Telemetry.DataContracts.Shared;
	using Relativity.Telemetry.Services.Metrics;

	[TestFixture]
	public class MetricSinkSumTest
	{
		private MetricSinkSum sumSink;
		private Mock<IServiceProxyFactory> mockServiceFactory;
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

			this.mockServiceFactory = new Mock<IServiceProxyFactory>();
			this.mockServiceFactory.Setup(foo => foo.CreateProxyInstance<IMetricsManager>()).Returns(mockMetricsManager.Object).Callback(() => this.createdProxy = true);
		}

		[Test]
		public void ShouldLogMetrics()
		{
			// Arrange
			var mockSumMetricFormatter = new Mock<ISumMetricFormatter>();
			mockSumMetricFormatter.Setup(foo => foo.GenerateSumMetrics(It.IsAny<MetricBase>()))
				.Returns(new List<MetricRef>() { new MetricRef(), new MetricRef(), new MetricRef() });
			this.sumSink = new MetricSinkSum(new KeplerProxyMock(this.mockServiceFactory.Object), mockSumMetricFormatter.Object, true);

			// Act
			this.sumSink.Log(new MetricJobStarted());

			// Assert
			Assert.True(this.createdProxy);
			Assert.AreEqual(3, this.loggedMetrics.Count);
		}

		[Test]
		public void ShouldNotCreateProxyIfListIsNull()
		{
			// Arrange
			var mockSumMetricFormatter = new Mock<ISumMetricFormatter>();
			mockSumMetricFormatter.Setup(foo => foo.GenerateSumMetrics(It.IsAny<MetricBase>()))
				.Returns((List<MetricRef>)null);
			this.sumSink = new MetricSinkSum(new KeplerProxyMock(this.mockServiceFactory.Object), mockSumMetricFormatter.Object, true);

			// Act
			this.sumSink.Log(new MetricJobStarted());

			// Assert
			Assert.False(this.createdProxy);
		}

		[Test]
		public void ShouldNotCreateProxyIfListIsEmpty()
		{
			// Arrange
			var mockSumMetricFormatter = new Mock<ISumMetricFormatter>();
			mockSumMetricFormatter.Setup(foo => foo.GenerateSumMetrics(It.IsAny<MetricBase>()))
				.Returns(new List<MetricRef>());
			this.sumSink = new MetricSinkSum(new KeplerProxyMock(this.mockServiceFactory.Object), mockSumMetricFormatter.Object, true);

			// Act
			this.sumSink.Log(new MetricJobStarted());

			// Assert
			Assert.False(this.createdProxy);
		}
	}
}