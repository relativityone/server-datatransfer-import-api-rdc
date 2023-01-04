// // ----------------------------------------------------------------------------
// <copyright file="SumMetricFormatterTest.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Monitoring;
	using Monitoring.Sinks;

	using Relativity.DataExchange.Transfer;

	[TestFixture]
	public class SumMetricFormatterTest
	{
		private SumMetricFormatter sumMetricFormatter;

		[SetUp]
		public void Setup()
		{
			this.sumMetricFormatter = new SumMetricFormatter();
		}

		[Test]
		public void ShouldFormatMetricJobStarted()
		{
			// Arrange
			var jobType = TelemetryConstants.TransferDirection.Import;
			var transferMode = TapiClient.Aspera;
			var metric = new MetricJobStarted() { TransferDirection = jobType, TransferMode = transferMode, WorkspaceID = 111111111 };

			// Act
			var result = this.sumMetricFormatter.GenerateSumMetrics(metric);

			// Assert
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual($"{TelemetryConstants.SumBucketPrefix.JOB_STARTED_COUNT}.{jobType}.{transferMode}", result[0].Bucket);
			Assert.AreEqual(new Guid("111111111fffffffffffffffffffffff"), result[0].WorkspaceGuid);
		}

		[TestCase(TelemetryConstants.TransferDirection.Import, 7)]
		[TestCase(TelemetryConstants.TransferDirection.Export, 10)]
		public void ShouldFormatMetricJobEndReport(TelemetryConstants.TransferDirection transferDirection, int numberOfGeneratedMetrics)
		{
			// Arrange
			var metric = new MetricJobEndReport() { TransferDirection = transferDirection };

			// Act
			var result = this.sumMetricFormatter.GenerateSumMetrics(metric);

			// Assert
			Assert.AreEqual(numberOfGeneratedMetrics, result.Count);
		}

		[Test]
		public void ShouldReturnEmptyListWhenUnsupportedMetric()
		{
			// Arrange
			var metric = new MetricJobProgress();

			// Act
			var result = this.sumMetricFormatter.GenerateSumMetrics(metric);

			// Assert
			Assert.IsEmpty(result);
		}
	}
}