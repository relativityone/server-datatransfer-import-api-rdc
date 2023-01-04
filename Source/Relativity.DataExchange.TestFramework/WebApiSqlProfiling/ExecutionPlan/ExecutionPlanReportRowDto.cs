// <copyright file="ExecutionPlanReportRowDto.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.ExecutionPlan
{
	using System.Collections.Generic;

	internal class ExecutionPlanReportRowDto
	{
		public ulong Duration { get; set; }

		public List<string> ExecutionPlans { get; set; }

		public string Sql { get; set; }

		public ulong QueryHash { get; set; }
	}
}
