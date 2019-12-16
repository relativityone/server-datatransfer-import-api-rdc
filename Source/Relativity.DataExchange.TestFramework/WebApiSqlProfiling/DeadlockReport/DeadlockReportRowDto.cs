// <copyright file="DeadlockReportRowDto.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.ExecutionPlan
{
	using System;
	using System.Collections.Generic;

	internal class DeadlockReportRowDto
	{
		public string DeadlockReport { get; set; }

		public List<string> LockObjectNames { get; } = new List<string>();
	}
}
