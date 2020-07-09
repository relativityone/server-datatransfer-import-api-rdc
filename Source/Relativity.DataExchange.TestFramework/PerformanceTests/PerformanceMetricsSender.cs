// <copyright file="PerformanceMetricsSender.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.PerformanceTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public static class PerformanceMetricsSender
	{
		public static void SendPerformanceMetrics(Dictionary<string, string> metricsWithValues)
		{
			// Temp solution till implementation of REL-423990 LoadTests: store metrics
			string parentFolder = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.FullName, "TestReports");
			string outputFile = Path.Combine(parentFolder, "PerformanceSummary.csv");
			Directory.CreateDirectory(parentFolder);

			metricsWithValues = metricsWithValues ?? throw new ArgumentNullException(nameof(metricsWithValues));

			if (!File.Exists(outputFile))
			{
				File.AppendAllText(outputFile, string.Join("|", metricsWithValues.Keys) + Environment.NewLine);
			}

			File.AppendAllText(outputFile, string.Join("|", metricsWithValues.Values) + Environment.NewLine);
		}
	}
}
