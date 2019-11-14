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
			var jobType = TelemetryConstants.JobType.Import;
			var transferMode = TapiClient.Aspera;
			var metric = new MetricJobStarted() { JobType = jobType, TransferMode = transferMode, WorkspaceID = 111111111 };

			// Act
			var result = this.sumMetricFormatter.GenerateSumMetrics(metric);

			// Assert
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual($"{TelemetryConstants.SumBucketPrefix.JOB_STARTED_COUNT}.{jobType}.{transferMode}", result[0].Bucket);
			Assert.AreEqual(new Guid("111111111fffffffffffffffffffffff"), result[0].WorkspaceGuid);
		}

		[TestCase(TelemetryConstants.JobType.Import, 7)]
		[TestCase(TelemetryConstants.JobType.Export, 6)]
		public void ShouldFormatMetricJobEndReport(TelemetryConstants.JobType jobType, int numberOfGeneratedMetrics)
		{
			// Arrange
			var metric = new MetricJobEndReport() { JobType = jobType };

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